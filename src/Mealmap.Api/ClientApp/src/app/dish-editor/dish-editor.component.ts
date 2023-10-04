import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, FormArray, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';
import { DishService } from '../services/dish.service';
import { Dish } from '../classes/dish';
import { IngredientFormData } from '../interfaces/ingredient-form-data';
import { DishFormData } from '../interfaces/dish-form-data';
import { SafeUrl } from '@angular/platform-browser';

type IngredientGroup = FormGroup<{
  quantity: FormControl<number | null>;
  unit: FormControl<string | null>;
  description: FormControl<string | null>;
}>;

@Component({
  selector: 'app-dish-editor',
  templateUrl: './dish-editor.component.html'
})
export class DishEditorComponent implements OnInit {
  private static readonly default_servings: number = 2;

  private _uneditedDish: Dish | undefined;
  private _image: Blob | undefined;
  private _localImageURL: SafeUrl | undefined;

  dish: Dish | undefined;

  form = new FormGroup({
    name: new FormControl<string | null>(null, [
      Validators.required,
      Validators.minLength(3),
      Validators.maxLength(50)
    ]),
    description: new FormControl<string | null>(null, Validators.maxLength(80)),
    servings: new FormControl<number>(DishEditorComponent.default_servings, Validators.min(1)),
    ingredients: new FormArray<IngredientGroup>([]),
    instructions: new FormControl<string | null>(null)
  });

  constructor(
    private dishService: DishService,
    private router: Router,
    private route: ActivatedRoute,
    private location: Location
  ) {}

  get ingredients(): FormArray {
    return this.form.controls['ingredients'] as FormArray<IngredientGroup>;
  }

  async ngOnInit(): Promise<void> {
    const id = this.route.snapshot.params['id'];

    if (id) this.dish = await this.dishService.getById(id);
    else this.dish = new Dish();

    this.InitializeValues();

    if (!this.dish?.id) this.disableControlsExceptName();

    this.addNameWatcher();
    this.addIngredientWatcher();
  }

  edited(): boolean {
    return this.form.dirty || this.dish?.image != this._image;
  }

  valid(): boolean {
    return this.form.valid;
  }

  hasName(): boolean {
    return this.form.get('name')!.valid;
  }

  hasImage(): boolean {
    return this._image != undefined;
  }

  imageURL(): SafeUrl | null {
    if (this._localImageURL) return this._localImageURL;
    return null;
  }

  isNew(): boolean {
    const url = this.route.snapshot.url;
    return url[url.length - 1].path == 'new';
  }

  onClickBack(): void {
    this.location.back();
  }

  async onClickSubmit(): Promise<void> {
    this.dish?.map(this.getFormData());

    const creating = this.dish?.id == undefined;

    if (this._image != undefined) this.dish?.setImage(this._image, this._localImageURL!);
    else this.dish?.deleteImage();

    this.dish = await this.dishService.save(this.dish!);

    if (creating) this.router.navigateByUrl(`dishes/${this.dish.id!}/edit`);
    else {
      this.form.markAsPristine();
      this.InitializeValues();
    }
  }

  onClickReset(): void {
    this.dish = this._uneditedDish;
    this._image = this.dish?.image;
    this._localImageURL = this.dish?.localImageURL;
    this.initializeFormValues();
  }

  async onClickDelete(): Promise<void> {
    await this.dishService.delete(this.dish!);
    this.router.navigateByUrl('dishes');
  }

  onImageSelected(event: Event): void {
    const target = event.target as HTMLInputElement;
    if (target.files) {
      this._image = target.files[0];
      this._localImageURL = this.dishService.urlFor(this._image);
    }
  }

  onClickDeleteImage(): void {
    this._image = undefined;
    this._localImageURL = undefined;
  }

  private disableControlsExceptName(): void {
    this.form.controls.description.disable({ emitEvent: false });
    this.form.controls.servings.disable({ emitEvent: false });
    this.form.controls.ingredients.disable({ emitEvent: false });
  }

  private enableControls(): void {
    const controls = this.form.controls;
    controls.description.enable({ emitEvent: false });
    controls.servings.enable({ emitEvent: false });
    controls.ingredients.enable({ emitEvent: false });
  }

  private InitializeValues(): void {
    this._image = this.dish?.image;
    this._localImageURL = this.dish?.localImageURL;
    this._uneditedDish = this.dish!.clone();
    this.initializeFormValues();
  }

  private initializeFormValues(): void {
    this.form.patchValue({
      name: this.dish?.name,
      description: this.dish?.description,
      servings: this.dish?.servings ?? DishEditorComponent.default_servings,
      instructions: this.dish?.instructions
    });
    this.initializeIngredientsForm();
  }

  private initializeIngredientsForm(): void {
    const ingredients = new FormArray<IngredientGroup>([]);

    this.dish?.ingredients?.forEach((ingredient) =>
      ingredients.push(
        new FormGroup({
          quantity: new FormControl<number | null>(ingredient.quantity, Validators.min(0)),
          unit: new FormControl<string | null>(ingredient.unitOfMeasurement),
          description: new FormControl<string | null>(ingredient.description)
        })
      )
    );
    this.form.setControl('ingredients', ingredients);
    this.addBlankIngredientFormGroup();
  }

  private addBlankIngredientFormGroup(): void {
    this.ingredients.push(
      new FormGroup({
        quantity: new FormControl<number | null>(null, Validators.min(0)),
        unit: new FormControl<string | null>(null),
        description: new FormControl<string | null>(null)
      })
    );
  }

  private isBlank(data: IngredientFormData): boolean {
    return (
      data.quantity == null &&
      (data.unit == null || data.unit.trim() == '') &&
      (data.description == null || data.description.trim() == '')
    );
  }

  private addNameWatcher(): void {
    this.form.valueChanges.subscribe(() => {
      if (this.form.get('name')?.valid) this.enableControls();
      else this.disableControlsExceptName();
    });
  }

  private addIngredientWatcher(): void {
    this.ingredients.valueChanges.subscribe(() => {
      const ingredients = this.ingredients.value as IngredientFormData[];
      const emptyRowCount = ingredients.filter((ingredient) => this.isBlank(ingredient)).length;

      if (emptyRowCount == 0) this.addBlankIngredientFormGroup();
      else if (emptyRowCount > 1) this.trimBlankIngredientFormGroup();
    });
  }

  private trimBlankIngredientFormGroup(): void {
    for (let i: number = 0; i < this.ingredients.length; i++) {
      if (this.isBlank(this.ingredients.at(i).value)) this.ingredients.removeAt(i);
    }
  }

  private getFormData(): DishFormData {
    const rawData = this.form.getRawValue();

    const formData: DishFormData = {
      name: rawData.name!,
      description: rawData.description ?? undefined,
      servings: rawData.servings!,
      ingredients: rawData.ingredients.filter(
        (ingredient) =>
          ingredient.quantity != null && ingredient.unit != null && ingredient.description != null
      ) as IngredientFormData[],
      instructions: rawData.instructions ?? undefined
    };

    return formData;
  }
}

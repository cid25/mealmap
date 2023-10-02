import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, FormArray, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
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
  selector: 'app-dish-details',
  templateUrl: './dish-details.component.html',
  styleUrls: ['./dish-details.component.css']
})
export class DishDetailsComponent implements OnInit {
  private static readonly default_servings: number = 2;

  private _editable: boolean = false;
  private _uneditedDish: Dish | undefined;
  private _image: Blob | undefined;
  private _localImageURL: SafeUrl | undefined;

  dish: Dish | undefined;

  form = new FormGroup({
    name: new FormControl<string | null>(null, [Validators.required, Validators.minLength(3)]),
    description: new FormControl<string | null>(null),
    servings: new FormControl<number>(DishDetailsComponent.default_servings, Validators.min(1)),
    ingredients: new FormArray<IngredientGroup>([])
  });

  constructor(
    private dishService: DishService,
    private route: ActivatedRoute,
    private location: Location
  ) {}

  get ingredients(): FormArray {
    return this.form.controls['ingredients'] as FormArray<IngredientGroup>;
  }

  async ngOnInit(): Promise<void> {
    const id = this.route.snapshot.params['id'];

    if (id) {
      this.dish = await this.dishService.getDish(id);
      if (this.route.snapshot.queryParams['edit']) this.enableEdit();
    } else {
      this.dish = new Dish();
      this.enableEdit();
      this.disableControlsExceptName();
    }

    this.InitializeValues();

    this.addNameWatcher();
    this.addIngredientWatcher();
  }

  back(): void {
    this.location.back();
  }

  editable(): boolean {
    return this._editable;
  }

  enableEdit(): void {
    this._editable = true;
  }

  disableEdit(): void {
    this._editable = false;
  }

  edited(): boolean {
    return this.form.dirty || this.dish?.image != this._image;
  }

  valid(): boolean {
    return this.form.valid;
  }

  submit(): void {
    this.form.value;
    const formData = this.form.getRawValue();

    const dishData: DishFormData = {
      name: formData.name!,
      description: formData.description ?? undefined,
      servings: formData.servings!,
      ingredients: formData.ingredients.filter(
        (ingredient) =>
          ingredient.quantity != null && ingredient.unit != null && ingredient.description != null
      ) as IngredientFormData[]
    };

    this.dish?.map(dishData);
  }

  reset(): void {
    this.dish = this._uneditedDish;
    this._image = this.dish?.image;
    this._localImageURL = this.dish?.localImageURL;
    this.initializeFormValues();
  }

  hasName(): boolean {
    return this.form.get('name')!.valid;
  }

  hasImage(): boolean {
    return !!this._image;
  }

  imageURL(): SafeUrl | null {
    if (this._localImageURL) return this._localImageURL;
    return null;
  }

  onImageSelected(event: Event): void {
    const target = event.target as HTMLInputElement;
    if (target.files) {
      this._image = target.files[0];
      this._localImageURL = this.dishService.getURLforImage(this._image);
    }
  }

  deleteImage(): void {
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
      servings: this.dish?.servings ?? DishDetailsComponent.default_servings
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

  private blank(data: IngredientFormData): boolean {
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
      const emptyRowCount = ingredients.filter((ingredient) => this.blank(ingredient)).length;

      if (emptyRowCount == 0) this.addBlankIngredientFormGroup();
      else if (emptyRowCount > 1) this.trimBlankIngredientFormGroup();
    });
  }

  private trimBlankIngredientFormGroup(): void {
    for (let i: number = 0; i < this.ingredients.length; i++) {
      if (this.blank(this.ingredients.at(i).value)) this.ingredients.removeAt(i);
    }
  }
}

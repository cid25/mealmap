import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, FormArray, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import { DishService } from '../services/dish.service';
import { Dish } from '../classes/dish';
import { IngredientFormData } from '../interfaces/ingredient-form-data';
import { DishFormData } from '../interfaces/dish-form-data';

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
    }

    this._uneditedDish = this.dish!.clone();

    this.initializeFormValues();
    this.ingredients.valueChanges.subscribe(() => {
      const ingredients = this.ingredients.value as IngredientFormData[];
      const emptyRowCount = ingredients.filter((ingredient) =>
        this.blankIngredientFormData(ingredient)
      ).length;
      if (emptyRowCount == 0) this.addBlankIngredientFormGroup();
      else if (emptyRowCount > 1) this.trimBlankIngredientFormGroup();
    });
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
    return JSON.stringify(this.dish) != JSON.stringify(this._uneditedDish);
  }

  hasImage(): boolean {
    return typeof this.dish?.image != 'undefined';
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
    console.log(this.dish);
  }

  private initializeFormValues(): void {
    this.form.patchValue({
      name: this.dish?.name,
      description: this.dish?.description,
      servings: this.dish?.servings ?? DishDetailsComponent.default_servings
    });

    this.initializeFormIngredients();
  }

  private initializeFormIngredients(): void {
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

  private blankIngredientFormData(data: IngredientFormData): boolean {
    console.log(data.quantity);
    return (
      data.quantity == null &&
      (data.unit == null || data.unit.trim() == '') &&
      (data.description == null || data.description.trim() == '')
    );
  }

  private trimBlankIngredientFormGroup(): void {
    const ingredients = new FormArray<IngredientGroup>([]);

    const ingredientControls = this.ingredients.controls;
    ingredientControls.forEach((control) => {
      if (!this.blankIngredientFormData(control.value))
        ingredients.push(control as IngredientGroup);
    });
    this.form.setControl('ingredients', ingredients);
    this.addBlankIngredientFormGroup();
  }
}

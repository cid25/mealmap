import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, FormArray } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import { DishService } from '../services/dish.service';
import { Dish } from '../classes/dish';

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
    name: new FormControl<string | null>(''),
    description: new FormControl<string | null>(''),
    servings: new FormControl<number | null>(DishDetailsComponent.default_servings),
    ingredients: new FormArray<IngredientGroup>([])
  });

  constructor(
    private dishService: DishService,
    private route: ActivatedRoute,
    private location: Location
  ) {}

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

  get ingredients(): FormArray {
    return this.form.controls['ingredients'] as FormArray<IngredientGroup>;
  }

  hasImage(): boolean {
    return typeof this.dish?.image != 'undefined';
  }

  quantity(index: number): FormControl<number | null> {
    const ingredientGroup = this.ingredients.controls[index] as IngredientGroup;
    return ingredientGroup.controls.quantity;
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
          quantity: new FormControl<number | null>(ingredient.quantity),
          unit: new FormControl<string | null>(ingredient.unitOfMeasurement),
          description: new FormControl<string | null>(ingredient.description)
        })
      )
    );

    this.addBlankIngredientFormGroup(ingredients);

    this.form.setControl('ingredients', ingredients);
  }

  private addBlankIngredientFormGroup(ingredientArray: FormArray<IngredientGroup>): void {
    ingredientArray.push(
      new FormGroup({
        quantity: new FormControl<number | null>(null),
        unit: new FormControl<string | null>(null),
        description: new FormControl<string | null>(null)
      })
    );
  }
}

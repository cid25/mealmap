import { IngredientFormData } from './ingredient-form-data';

export interface DishFormData {
  name: string;
  description: string | undefined;
  servings: number;
  ingredients: IngredientFormData[];
}

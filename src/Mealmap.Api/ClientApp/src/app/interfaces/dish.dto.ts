import { IngredientDTO } from './ingredient.dto';

export interface DishDTO {
  id: string;
  eTag: string;
  name: string;
  description?: string;
  imageUrl: string;
  servings: number;
  ingredients: IngredientDTO[];
  instructions?: string;
}

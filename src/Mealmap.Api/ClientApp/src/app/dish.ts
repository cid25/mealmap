import { Ingredient } from "./ingredient";

export interface Dish {
  id: string;
  eTag: string;
  name: string;
  description: string;
  servings: number;
  ingredients: Ingredient[];
}

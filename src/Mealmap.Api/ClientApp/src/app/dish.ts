import { Ingredient } from "./ingredient";
import { SafeUrl } from "@angular/platform-browser";

export interface Dish {
  id: string;
  eTag: string;
  name: string;
  description: string;
  servings: number;
  ingredients: Ingredient[];
  image: Blob;
  localImageURL: SafeUrl;
}

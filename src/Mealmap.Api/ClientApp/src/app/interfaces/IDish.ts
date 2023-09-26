import { IIngredient } from './IIngredient';
import { SafeUrl } from '@angular/platform-browser';

export interface IDish {
  id: string;
  eTag: string;
  name: string;
  description: string;
  servings: number;
  ingredients: IIngredient[];
  image: Blob;
  localImageURL: SafeUrl;
}

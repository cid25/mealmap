import { SafeUrl } from '@angular/platform-browser';
import { Ingredient } from './ingredient';
import { DishDTO } from '../interfaces/dish.dto';

export class Dish {
  id?: string;
  eTag?: string;
  name?: string;
  description?: string;
  servings?: number;
  ingredients?: Ingredient[];
  imageUrl?: string;
  image?: Blob;
  localImageURL?: SafeUrl;

  static from(dto: DishDTO) {
    const result = new Dish();
    return result.copy(dto);
  }

  private copy(dto: DishDTO): Dish {
    this.id = dto.id;
    this.eTag = dto.eTag;
    this.name = dto.name;
    this.description = dto.description;
    this.servings = dto.servings;
    this.imageUrl = dto.imageUrl;
    this.ingredients = dto.ingredients.map((data) => Ingredient.from(data));
    return this;
  }
}

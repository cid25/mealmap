import { SafeUrl } from '@angular/platform-browser';
import { Ingredient } from './ingredient';
import { DishDTO } from '../interfaces/dish.dto';
import { DishFormData } from '../interfaces/dish-form-data';

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

  clone(): Dish {
    const result = new Dish();
    result.id = this.id;
    result.eTag = this.eTag;
    result.name = this.name;
    result.description = this.description;
    result.servings = this.servings;
    result.ingredients = this.ingredients?.map((original) => original.clone());
    result.imageUrl = this.imageUrl;
    result.image = this.image;
    result.localImageURL = this.localImageURL;
    return result;
  }

  map(data: DishFormData): Dish {
    this.name = data.name!;
    this.description = data.description;
    this.servings = data.servings;
    this.ingredients = data.ingredients.map((data) => Ingredient.fromFormData(data));
    return this;
  }

  private copy(dto: DishDTO): Dish {
    this.id = dto.id;
    this.eTag = dto.eTag;
    this.name = dto.name;
    this.description = dto.description;
    this.servings = dto.servings;
    this.ingredients = dto.ingredients.map((data) => Ingredient.fromDTO(data));
    this.imageUrl = dto.imageUrl;
    return this;
  }
}

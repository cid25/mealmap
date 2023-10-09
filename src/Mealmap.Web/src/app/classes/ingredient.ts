import { IngredientFormData } from '../interfaces/ingredient-form-data';
import { IngredientDTO } from '../interfaces/ingredient.dto';

export class Ingredient {
  quantity: number;
  unitOfMeasurement: string;
  description: string;

  constructor(quantity: number, unitOfMeasurement: string, description: string) {
    this.quantity = quantity;
    this.unitOfMeasurement = unitOfMeasurement;
    this.description = description;
  }

  static fromDTO(dto: IngredientDTO): Ingredient {
    const result = new Ingredient(dto.quantity, dto.unitOfMeasurement, dto.description);
    return result;
  }

  static fromFormData(data: IngredientFormData) {
    const result = new Ingredient(data.quantity, data.unit, data.description);
    return result;
  }

  clone(): Ingredient {
    return new Ingredient(this.quantity, this.unitOfMeasurement, this.description);
  }
}

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

  static from(dto: IngredientDTO): Ingredient {
    const result = new Ingredient(dto.quantity, dto.unitOfMeasurement, dto.description);
    return result;
  }

  clone(): Ingredient {
    return new Ingredient(this.quantity, this.unitOfMeasurement, this.description);
  }
}

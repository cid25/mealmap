import { Dish } from '../domain/dish';

export interface DishPickedEvent {
  index: number;
  dish: Dish;
}

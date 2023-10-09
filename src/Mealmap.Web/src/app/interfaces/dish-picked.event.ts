import { Dish } from '../classes/dish';

export interface DishPickedEvent {
  index: number;
  dish: Dish;
}

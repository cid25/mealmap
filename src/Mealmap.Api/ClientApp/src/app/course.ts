import { Dish } from './dish';

export interface Course {
  index: number;
  dishId: string;
  mainCourse: boolean;
  dish: Dish;
}

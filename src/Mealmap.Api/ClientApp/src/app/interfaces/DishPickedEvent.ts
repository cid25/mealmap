import { IDish } from './IDish';

export interface DishPickedEvent {
  index: number;
  dish: IDish;
}

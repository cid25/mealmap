import { IDish } from './IDish';

export interface ICourse {
  index: number;
  dishId: string;
  dish?: IDish;
  mainCourse: boolean;
}

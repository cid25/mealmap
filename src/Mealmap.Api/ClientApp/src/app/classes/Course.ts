import { ICourse } from '../interfaces/ICourse';
import { IDish } from '../interfaces/IDish';

export class Course {
  index: number;
  dishId: string;
  dish?: IDish;
  mainCourse: boolean = false;

  constructor(index: number, dishId: string) {
    this.index = index;
    this.dishId = dishId;
  }

  copy(original: ICourse): Course {
    this.index = original.index;
    this.dishId = original.dishId;
    this.mainCourse = original.mainCourse;
    if (original.dish) this.dish = original.dish;
    return this;
  }

  clone(): Course {
    const clone = new Course(this.index, this.dishId);
    if (this.dish) clone.dish = this.dish;
    clone.mainCourse = this.mainCourse;
    return clone;
  }
}

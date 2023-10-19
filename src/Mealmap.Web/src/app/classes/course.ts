import { CourseDTO } from '../interfaces/course.dto';
import { Dish } from './dish';

export class Course {
  index: number;
  dishId: string;
  dish?: Dish;
  mainCourse: boolean = false;
  attendees: number;

  constructor(index: number, dishId: string, attendees: number) {
    this.index = index;
    this.dishId = dishId;
    this.attendees = attendees;
  }

  copy(dto: CourseDTO): Course {
    this.index = dto.index;
    this.dishId = dto.dishId;
    this.mainCourse = dto.mainCourse;
    this.attendees = dto.attendees;
    return this;
  }

  clone(): Course {
    const clone = new Course(this.index, this.dishId, this.attendees);
    if (this.dish) clone.dish = this.dish;
    clone.mainCourse = this.mainCourse;
    return clone;
  }

  static sort(a: Course, b: Course): number {
    return a.index - b.index;
  }
}

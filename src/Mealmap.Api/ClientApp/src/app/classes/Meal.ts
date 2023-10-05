import { MealDTO } from '../interfaces/meal.dto';
import { Course } from './course';
import { DateTime } from 'luxon';

export class Meal {
  id?: string;
  eTag?: string;
  diningDate: Date;
  courses: Course[] = [];

  constructor(diningDate: Date) {
    this.diningDate = diningDate;
  }

  clone(): Meal {
    const clone = new Meal(this.diningDate);
    clone.id = this.id;
    clone.eTag = this.eTag;
    clone.courses = this.courses.map((course) => course.clone());
    return clone;
  }

  key(): string {
    return Meal.keyFor(this.diningDate);
  }

  toJSON(): string {
    const result = {
      id: this.id ?? null,
      diningDate: DateTime.fromJSDate(this.diningDate).toISODate(),
      courses: this.courses.map((course: Course) => {
        const clone = course.clone();
        delete clone.dish;
        return clone;
      })
    };

    return JSON.stringify(result);
  }

  static keyFor(date: Date): string {
    const dateString =
      date.getFullYear() +
      '-' +
      ('0' + (date.getMonth() + 1).toString()).slice(-2) +
      '-' +
      ('0' + date.getDate().toString()).slice(-2);

    return dateString;
  }

  static from(dto: MealDTO) {
    const dateFromString = DateTime.fromISO(dto.diningDate).toJSDate();
    const result = new Meal(dateFromString);
    return result.copy(dto);
  }

  private copy(dto: MealDTO): Meal {
    this.id = dto.id;
    this.eTag = dto.eTag;
    this.courses = dto.courses
      .map((data) => {
        const courseCopy = new Course(data.index, data.dishId);
        courseCopy.copy(data);
        return courseCopy;
      })
      .sort(Course.sort);
    return this;
  }
}

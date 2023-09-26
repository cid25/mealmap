import { IMeal } from '../interfaces/IMeal';
import { Course } from './Course';
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

  static keyFor(date: Date): string {
    const dateString =
      date.getFullYear() +
      '-' +
      ('0' + (date.getMonth() + 1).toString()).slice(-2) +
      '-' +
      ('0' + date.getDate().toString()).slice(-2);

    return dateString;
  }

  static from(data: IMeal) {
    const dateFromString = DateTime.fromISO(data.diningDate).toJSDate();
    const meal = new Meal(dateFromString);
    return meal.copy(data);
  }

  private copy(original: IMeal): Meal {
    this.id = original.id;
    this.eTag = original.eTag;
    this.courses = original.courses.map((orig) => {
      const courseCopy = new Course(orig.index, orig.dishId);
      courseCopy.copy(orig);
      return courseCopy;
    });
    return this;
  }
}

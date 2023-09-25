import { Course } from './course';

export interface Meal {
  id: string;
  eTag: string;
  diningDate: Date;
  courses: Course[];
}

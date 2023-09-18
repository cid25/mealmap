import { Course } from "./course";

export interface Meal {
  id: string;
  diningDate: Date;
  courses: Course[];
}

import { CourseDTO } from './course.dto';

export interface MealDTO {
  id?: string;
  eTag?: string;
  diningDate: string;
  courses: CourseDTO[];
}

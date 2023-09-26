import { ICourse } from './ICourse';

export interface IMeal {
  id?: string;
  eTag?: string;
  diningDate: string;
  courses: ICourse[];
}

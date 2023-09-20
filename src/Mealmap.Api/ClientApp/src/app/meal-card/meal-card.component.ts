import { Component, Input } from '@angular/core';
import { Meal } from '../interfaces/meal';
import { Course } from '../interfaces/course';

@Component({
  selector: 'app-meal-card',
  templateUrl: './meal-card.component.html',
  styleUrls: ['./meal-card.component.css']
})
export class MealCardComponent {
  @Input() meal: Meal | undefined;

  hasCourses(): boolean {
    if (this.meal?.courses && this.meal?.courses?.length > 0) return true;
    else return false;
  }

  mainCourse(): Course | null {
    if (this.meal?.courses.length == 1) {
      return this.meal.courses[0];
    }

    const mainCourse = this.meal?.courses.find((course) => course.mainCourse);
    if (mainCourse !== undefined) return mainCourse;

    return null;
  }
}

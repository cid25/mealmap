import { Component, Input } from '@angular/core';
import { Meal } from "../meal";
import { Dish } from "../dish";
import { Course } from '../course';

@Component({
  selector: 'app-meal-card',
  templateUrl: './meal-card.component.html',
  styleUrls: ['./meal-card.component.css']
})
export class MealCardComponent {
  @Input() meal: Meal | undefined;

  mainCourse(): Course | null {
    if (this.meal?.courses.length == 1) {
      return this.meal.courses[0];
    }

    const mainCourse = this.meal?.courses.find(course => course.mainCourse);
    if (mainCourse !== undefined)
      return mainCourse;

    return null;
  }
}

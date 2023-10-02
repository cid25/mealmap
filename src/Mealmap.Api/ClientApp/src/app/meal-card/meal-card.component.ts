import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Meal } from '../classes/meal';
import { Course } from '../classes/course';
import { SafeUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-meal-card',
  templateUrl: './meal-card.component.html',
  styleUrls: ['./meal-card.component.css']
})
export class MealCardComponent {
  @Input()
  meal!: Meal;

  @Output()
  deleted = new EventEmitter();

  hasCourses(): boolean {
    if (this.meal?.courses && this.meal?.courses?.length > 0) return true;
    else return false;
  }

  mainCourse(): Course | null {
    if (this.meal?.courses.length == 1) return this.meal.courses[0];

    const mainCourse = this.meal?.courses.find((course) => course.mainCourse);
    if (mainCourse !== undefined) return mainCourse;

    return null;
  }

  hasImage(): boolean {
    const mainCourse = this.mainCourse();
    return !!mainCourse && !!mainCourse.dish?.localImageURL;
  }
}

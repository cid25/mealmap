import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Meal } from 'src/app/domain/meal';
import { Course } from 'src/app/domain/course';

@Component({
  selector: 'app-meal-schedule-card',
  templateUrl: './meal-schedule-card.component.html'
})
export class MealScheduleCardComponent {
  @Input()
  meal!: Meal;

  @Input()
  highlight: boolean = false;

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

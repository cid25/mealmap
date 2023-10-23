import { Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DateTime } from 'luxon';
import { Meal } from '../../domain/meal';
import { MealService } from '../../services/meal.service';
import { Course } from '../../domain/course';

@Component({
  selector: 'app-meal-viewer',
  templateUrl: './meal-viewer.component.html'
})
export class MealViewerComponent implements OnInit {
  meal: Meal | undefined;

  constructor(
    private mealService: MealService,
    private route: ActivatedRoute,
    private location: Location
  ) {}

  get diningDate(): Date {
    if (this.meal == undefined) return new Date();
    else return this.meal.diningDate;
  }

  get courses(): Course[] {
    if (this.meal === undefined) return [];
    return this.meal!.courses.sort((a, b) => a.index - b.index);
  }

  async ngOnInit(): Promise<void> {
    const diningDate = DateTime.fromISO(this.route.snapshot.params['date']).toJSDate();
    this.meal = await this.mealService.getMealFor(diningDate);
  }

  onClickBack(): void {
    this.location.back();
  }

  mainCourse(course: Course): boolean {
    return course.mainCourse;
  }
}

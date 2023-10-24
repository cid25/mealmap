import { Location } from '@angular/common';
import { Component, OnChanges, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DateTime } from 'luxon';
import { Course } from 'src/app/domain/course';
import { Meal } from 'src/app/domain/meal';
import { DishPickedEvent } from 'src/app/events/dish-picked.event';
import { CourseEvent } from 'src/app/components/meal-editor-card/meal-editor-card.component';
import { MealService } from 'src/app/services/meal.service';

@Component({
  selector: 'app-meal-editor',
  templateUrl: './meal-editor.component.html'
})
export class MealEditorComponent implements OnInit, OnChanges {
  private uneditedMeal: Meal | undefined;

  meal: Meal | undefined;

  dishPickerActive: boolean = false;
  courseIndexPicking: number = 0;

  diningDate: Date;

  constructor(
    private mealService: MealService,
    private route: ActivatedRoute,
    private location: Location
  ) {
    this.diningDate = DateTime.fromISO(this.route.snapshot.params['date']).toJSDate();
  }

  get courses(): Course[] {
    if (this.meal === undefined) return [];
    return this.meal.courses;
  }

  async ngOnInit(): Promise<void> {
    await this.retrieveMeal();
  }

  async ngOnChanges(): Promise<void> {
    this.deactivatePicker();
    await this.retrieveMeal();
  }

  back(): void {
    this.location.back();
  }

  mealEdited(): boolean {
    if (this.meal == undefined) return false;
    return this.meal.toJSON() != this.uneditedMeal!.toJSON();
  }

  nextCourseIndex(): number {
    const indices = this.meal?.courses.map<number>((course) => course.index);
    if (indices != undefined && indices.length > 0) return Math.max(...indices) + 1;
    else return 1;
  }

  async saveChanges(): Promise<void> {
    await this.mealService.saveMeal(this.meal!);
    this.retrieveMeal();
  }

  discardChanges(): void {
    this.meal = this.uneditedMeal?.clone();
    this.deactivatePicker();
  }

  onPickerActivated(event: CourseEvent): void {
    this.activatePicker(event.index);
  }

  activatePicker(index: number): void {
    this.dishPickerActive = true;
    this.courseIndexPicking = index;
  }

  cancelPicking(): void {
    this.deactivatePicker();
  }

  pickDish(event: DishPickedEvent): void {
    this.deactivatePicker();
    const pickedEvent = event as unknown as DishPickedEvent;
    const existingCourse = this.meal?.courses.filter((course) => course.index == pickedEvent.index);
    if (existingCourse?.length === 1) {
      existingCourse[0].dish = pickedEvent.dish;
      existingCourse[0].dishId = pickedEvent.dish.id!;
    } else {
      const course = new Course(pickedEvent.index, pickedEvent.dish.id!, 1);
      course.dish = pickedEvent.dish;
      if (this.meal?.courses.length === 0) course.mainCourse = true;
      this.meal?.courses.push(course);
    }
  }

  onCoursePromoted(event: CourseEvent): void {
    this.meal?.courses.forEach((course) => {
      if (course.index == event.index) course.mainCourse = true;
      else course.mainCourse = false;
    });
  }

  onCourseDeleted(event: CourseEvent): void {
    const remainingCourses = this.meal?.courses.filter((course) => course.index != event.index);
    if (remainingCourses == undefined) this.meal!.courses = [];
    else this.meal!.courses = remainingCourses;

    this.shiftCourses(event.index);

    if (this.meal?.courses.length == 1) this.meal.courses[0].mainCourse = true;
  }

  private async retrieveMeal(): Promise<void> {
    const meal = await this.mealService.getMealFor(this.diningDate);
    this.meal = meal;
    this.uneditedMeal = meal.clone();
  }

  private deactivatePicker(): void {
    this.dishPickerActive = false;
  }

  private shiftCourses(indexRemoved: number): void {
    this.meal?.courses.forEach(
      (course) => (course.index = course.index > indexRemoved ? --course.index : course.index)
    );
  }
}

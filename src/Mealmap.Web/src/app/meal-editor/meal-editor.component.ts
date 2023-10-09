import { Component, OnChanges, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';
import { DateTime } from 'luxon';
import { MealService } from '../services/meal.service';
import { DishPickedEvent } from '../interfaces/dish-picked.event';
import { Meal } from '../classes/meal';
import { Course } from '../classes/course';

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
    private router: Router,
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

  mainCourse(course: Course): boolean {
    return course.mainCourse;
  }

  makeMainCourse(index: number) {
    this.meal?.courses.forEach((course) => {
      if (course.index == index) course.mainCourse = true;
      else course.mainCourse = false;
    });
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

  deleteCourse(index: number): void {
    const remainingCourses = this.meal?.courses.filter((course) => course.index != index);
    if (remainingCourses == undefined) this.meal!.courses = [];
    else this.meal!.courses = remainingCourses;

    this.shiftCourses(index);

    if (this.meal?.courses.length == 1) this.meal.courses[0].mainCourse = true;
  }

  async saveChanges(): Promise<void> {
    await this.mealService.saveMeal(this.meal!);
    this.retrieveMeal();
  }

  discardChanges(): void {
    this.meal = this.uneditedMeal?.clone();
    this.deactivatePicker();
  }

  activatePicker(courseIndex: number): void {
    this.dishPickerActive = true;
    this.courseIndexPicking = courseIndex;
  }

  cancelPicking(): void {
    this.deactivatePicker();
  }

  pickDish(event: DishPickedEvent): void {
    this.deactivatePicker();
    const existingCourse = this.meal?.courses.filter((course) => course.index == event.index);
    if (existingCourse?.length === 1) {
      existingCourse[0].dish = event.dish;
      existingCourse[0].dishId = event.dish.id!;
    } else {
      const course = new Course(event.index, event.dish.id!);
      course.dish = event.dish;
      if (this.meal?.courses.length === 0) course.mainCourse = true;
      this.meal?.courses.push(course);
    }
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
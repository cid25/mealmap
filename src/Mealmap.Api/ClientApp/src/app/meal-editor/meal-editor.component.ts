import { Component, Input, Output, EventEmitter, OnChanges, OnInit } from '@angular/core';
import { MealService } from '../services/meal.service';
import { Meal } from '../interfaces/meal';
import { Course } from '../interfaces/course';

@Component({
  selector: 'app-meal-editor',
  templateUrl: './meal-editor.component.html',
  styleUrls: ['./meal-editor.component.css']
})
export class MealEditorComponent implements OnInit, OnChanges {
  private intialMealFlattened: string = '';

  meal: Meal | undefined;

  dishPickerActive: boolean = false;
  courseIndexPicking: number = 0;

  @Input()
  diningDate!: Date;

  @Output()
  editStopped = new EventEmitter();

  constructor(private mealService: MealService) {}

  async ngOnInit(): Promise<void> {
    this.meal = await this.mealService.getMealFor(this.diningDate);
    this.intialMealFlattened = JSON.stringify(this.meal);
  }

  async ngOnChanges(): Promise<void> {
    this.meal = await this.mealService.getMealFor(this.diningDate);
    this.intialMealFlattened = JSON.stringify(this.meal);
    this.deactivatePicker();
  }

  stopEdit(): void {
    this.editStopped.emit(this.diningDate);
  }

  coursesForDisplay(): Course[] {
    if (this.meal === undefined) return [];
    return this.meal!.courses.sort();
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
    return this.intialMealFlattened != JSON.stringify(this.meal);
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

  activatePicker(courseIndex: number): void {
    this.dishPickerActive = true;
    this.courseIndexPicking = courseIndex;
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

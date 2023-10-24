import { Component, Output, EventEmitter, Input, OnInit } from '@angular/core';
import { Course } from 'src/app/domain/course';
import { FormControl } from '@angular/forms';

export type CourseEvent = {
  index: number;
};

@Component({
  selector: 'app-meal-editor-card',
  templateUrl: './meal-editor-card.component.html'
})
export class MealEditorCardComponent implements OnInit {
  @Input()
  course!: Course;

  attendeesControl = new FormControl<number>(0);

  @Output() pickerActivated = new EventEmitter<CourseEvent>();
  @Output() coursePromoted = new EventEmitter<CourseEvent>();
  @Output() courseDeleted = new EventEmitter<CourseEvent>();

  ngOnInit(): void {
    this.attendeesControl.setValue(this.course.attendees, { emitEvent: false });
    this.attendeesControl.valueChanges.subscribe(() => {
      this.course.attendees = this.attendeesControl.value ?? 0;
    });
  }

  activatePicker(): void {
    this.pickerActivated.emit({ index: this.course.index });
  }

  makeMainCourse() {
    this.coursePromoted.emit({ index: this.course.index });
  }

  deleteCourse(): void {
    this.courseDeleted.emit({ index: this.course.index });
  }
}

import { Component } from '@angular/core';

@Component({
  selector: 'app-meal-page',
  templateUrl: './meal-page.component.html',
  styleUrls: ['./meal-page.component.css']
})
export class MealPageComponent {
  private inEditMode: boolean = false;
  private dateEditable!: Date;

  startEdit(date: Date): void {
    this.inEditMode = true;
    this.dateEditable = date;
  }

  underEdit(): boolean {
    return this.inEditMode;
  }

  editableDate(): Date {
    return this.dateEditable;
  }
}

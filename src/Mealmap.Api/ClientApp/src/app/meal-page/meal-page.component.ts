import { Component } from '@angular/core';

@Component({
  selector: 'app-meal-page',
  templateUrl: './meal-page.component.html',
  styleUrls: ['./meal-page.component.css']
})
export class MealPageComponent {
  private inEditMode: boolean = false;
  private dateEditable!: Date | undefined;

  startEdit(date: Date): void {
    this.inEditMode = true;
    this.dateEditable = date;
  }

  stopEdit(): void {
    this.inEditMode = false;
    this.dateEditable = undefined;
  }

  underEdit(): boolean {
    return this.inEditMode;
  }

  editableDate(): Date {
    if (this.dateEditable !== undefined) return this.dateEditable;
    else return new Date();
  }
}

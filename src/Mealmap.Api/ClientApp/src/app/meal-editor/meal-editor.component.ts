import { Component, Input } from '@angular/core';
import { MealService } from '../services/meal.service';

@Component({
  selector: 'app-meal-editor',
  templateUrl: './meal-editor.component.html',
  styleUrls: ['./meal-editor.component.css']
})
export class MealEditorComponent {
  @Input()
  diningDate!: Date;

  constructor(private mealService: MealService) {}
}

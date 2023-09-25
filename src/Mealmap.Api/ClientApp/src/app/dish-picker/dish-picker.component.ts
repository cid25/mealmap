import { Component, Input } from '@angular/core';
import { DishService } from '../services/dish.service';
import { Dish } from '../interfaces/dish';

@Component({
  selector: 'app-dish-picker',
  templateUrl: './dish-picker.component.html',
  styleUrls: ['./dish-picker.component.css']
})
export class DishPickerComponent {
  constructor(private dishService: DishService) {}

  @Input()
  index: number = 0;

  async getDishes(): Promise<Dish[]> {
    return this.dishService.getDishes([
      'da7e7a58-4b29-4e26-80ee-35dd9f13a97d',
      '66334f32-78d6-4d72-a03f-b8e3403fe690'
    ]);
  }
}

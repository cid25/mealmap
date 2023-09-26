import { Component, Input, OnInit } from '@angular/core';
import { DishService } from '../services/dish.service';
import { Dish } from '../interfaces/dish';

@Component({
  selector: 'app-dish-picker',
  templateUrl: './dish-picker.component.html',
  styleUrls: ['./dish-picker.component.css']
})
export class DishPickerComponent implements OnInit {
  constructor(private dishService: DishService) {}

  dishes: Dish[] = [];

  @Input()
  index: number = 0;

  async ngOnInit(): Promise<void> {
    this.dishes = await this.dishService.getDishes([
      'da7e7a58-4b29-4e26-80ee-35dd9f13a97d',
      '66334f32-78d6-4d72-a03f-b8e3403fe690'
    ]);
  }
}

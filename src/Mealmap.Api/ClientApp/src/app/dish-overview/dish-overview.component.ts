import { Component, OnInit } from '@angular/core';
import { Dish } from '../classes/dish';
import { DishService } from '../services/dish.service';

@Component({
  selector: 'app-dish-overview',
  templateUrl: './dish-overview.component.html',
  styleUrls: ['./dish-overview.component.css']
})
export class DishOverviewComponent implements OnInit {
  private _dishes: Dish[] = [];

  constructor(private dishService: DishService) {}

  async ngOnInit(): Promise<void> {
    this._dishes = await this.dishService.listDishes();
  }

  dishesForDisplay(): Dish[] {
    return this._dishes;
  }
}

import { Component, OnInit } from '@angular/core';
import { IDish } from '../interfaces/IDish';
import { DishService } from '../services/dish.service';

@Component({
  selector: 'app-dish-overview',
  templateUrl: './dish-overview.component.html',
  styleUrls: ['./dish-overview.component.css']
})
export class DishOverviewComponent implements OnInit {
  private _dishes: IDish[] = [];

  constructor(private dishService: DishService) {}

  async ngOnInit(): Promise<void> {
    this._dishes = await this.dishService.getDishes([
      'da7e7a58-4b29-4e26-80ee-35dd9f13a97d',
      '66334f32-78d6-4d72-a03f-b8e3403fe690'
    ]);
  }

  dishesForDisplay(): IDish[] {
    return this._dishes;
  }
}

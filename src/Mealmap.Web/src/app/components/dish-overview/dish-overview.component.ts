import { Component, OnInit } from '@angular/core';
import { Dish } from 'src/app/domain/dish';
import { DishService } from 'src/app/services/dish.service';
import { SearchedEvent } from 'src/app/events/searched.event';

@Component({
  selector: 'app-dish-overview',
  templateUrl: './dish-overview.component.html'
})
export class DishOverviewComponent implements OnInit {
  private _dishes: Dish[] = [];

  constructor(private dishService: DishService) {}

  async ngOnInit(): Promise<void> {
    this._dishes = await this.dishService.get(50);
  }

  dishesForDisplay(): Dish[] {
    return this._dishes;
  }

  async onSearch(event: SearchedEvent): Promise<void> {
    this._dishes = await this.dishService.get(undefined, event.searchterm);
  }
}

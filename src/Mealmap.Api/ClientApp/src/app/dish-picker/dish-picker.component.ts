import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { DishService } from '../services/dish.service';
import { Dish } from '../classes/dish';
import { DishPickedEvent } from '../interfaces/dish-picked.event';

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

  @Output()
  picked = new EventEmitter<DishPickedEvent>();

  @Output()
  cancelled = new EventEmitter();

  async ngOnInit(): Promise<void> {
    this.dishes = await this.dishService.get();
  }

  async select(id: string): Promise<void> {
    const dishPicked = await this.dishService.getById(id);
    this.picked.emit({ index: this.index, dish: dishPicked! });
  }

  cancel(): void {
    this.cancelled.emit();
  }
}

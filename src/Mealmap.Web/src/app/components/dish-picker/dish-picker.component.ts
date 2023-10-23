import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { DishService } from '../../services/dish.service';
import { Dish } from '../../domain/dish';
import { SearchedEvent } from '../../events/searched.event';
import { DishPickedEvent } from '../../events/dish-picked.event';

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

  onClickCancel(): void {
    this.cancelled.emit();
  }

  async onSearch(event: SearchedEvent): Promise<void> {
    this.dishes = await this.dishService.get(undefined, event.searchterm);
  }
}

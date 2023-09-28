import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { DishService } from '../services/dish.service';
import { IDish } from '../interfaces/IDish';
import { DishPickedEvent } from '../interfaces/DishPickedEvent';

@Component({
  selector: 'app-dish-picker',
  templateUrl: './dish-picker.component.html',
  styleUrls: ['./dish-picker.component.css']
})
export class DishPickerComponent implements OnInit {
  constructor(private dishService: DishService) {}

  dishes: IDish[] = [];

  @Input()
  index: number = 0;

  @Output()
  picked = new EventEmitter<DishPickedEvent>();

  @Output()
  cancelled = new EventEmitter();

  async ngOnInit(): Promise<void> {
    this.dishes = await this.dishService.getDishes([
      'da7e7a58-4b29-4e26-80ee-35dd9f13a97d',
      '66334f32-78d6-4d72-a03f-b8e3403fe690'
    ]);
  }

  async select(id: string): Promise<void> {
    const dishPicked = await this.dishService.getDish(id);
    this.picked.emit({ index: this.index, dish: dishPicked! });
  }

  cancel(): void {
    this.cancelled.emit();
  }
}

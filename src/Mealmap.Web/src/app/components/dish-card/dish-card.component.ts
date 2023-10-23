import { Component, Input } from '@angular/core';
import { Dish } from '../../domain/dish';

@Component({
  selector: 'app-dish-card',
  templateUrl: './dish-card.component.html'
})
export class DishCardComponent {
  @Input()
  dish!: Dish;
}

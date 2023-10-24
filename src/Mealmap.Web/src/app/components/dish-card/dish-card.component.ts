import { Component, Input } from '@angular/core';
import { Dish } from 'src/app/domain/dish';

@Component({
  selector: 'app-dish-card',
  templateUrl: './dish-card.component.html'
})
export class DishCardComponent {
  @Input()
  dish!: Dish;
}

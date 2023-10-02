import { Component, Input } from '@angular/core';
import { Dish } from '../classes/dish';

@Component({
  selector: 'app-dish-card',
  templateUrl: './dish-card.component.html',
  styleUrls: ['./dish-card.component.css']
})
export class DishCardComponent {
  @Input()
  dish!: Dish;

  hasImage(): boolean {
    return !!this.dish.image;
  }
}

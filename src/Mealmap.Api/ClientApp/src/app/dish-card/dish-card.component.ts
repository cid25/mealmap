import { Component, Input } from '@angular/core';
import { IDish } from '../interfaces/IDish';

@Component({
  selector: 'app-dish-card',
  templateUrl: './dish-card.component.html',
  styleUrls: ['./dish-card.component.css']
})
export class DishCardComponent {
  @Input()
  dish!: IDish;
}

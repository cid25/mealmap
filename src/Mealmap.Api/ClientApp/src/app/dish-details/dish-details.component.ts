import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DishService } from '../services/dish.service';
import { Dish } from '../classes/dish';

@Component({
  selector: 'app-dish-details',
  templateUrl: './dish-details.component.html',
  styleUrls: ['./dish-details.component.css']
})
export class DishDetailsComponent implements OnInit {
  dish: Dish | undefined;

  constructor(
    private dishService: DishService,
    private route: ActivatedRoute
  ) {}

  async ngOnInit(): Promise<void> {
    const id = this.route.snapshot.params['id'];

    if (id) this.dish = await this.dishService.getDish(id);
    else this.dish = new Dish();
  }
}

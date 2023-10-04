import { Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Dish } from '../classes/dish';
import { DishService } from '../services/dish.service';

@Component({
  selector: 'app-dish-viewer',
  templateUrl: './dish-viewer.component.html'
})
export class DishViewerComponent implements OnInit {
  dish: Dish | undefined;

  constructor(
    private dishService: DishService,
    private router: Router,
    private route: ActivatedRoute,
    private location: Location
  ) {}

  async ngOnInit(): Promise<void> {
    const id = this.route.snapshot.params['id'];

    this.dish = await this.dishService.getById(id);
  }

  onClickBack(): void {
    this.location.back();
  }

  hasIngredients(): boolean {
    return (
      this.dish != undefined &&
      this.dish.ingredients != undefined &&
      this.dish.ingredients?.length > 0
    );
  }
}
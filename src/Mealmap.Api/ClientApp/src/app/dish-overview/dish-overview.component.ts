import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Dish } from '../classes/dish';
import { DishService } from '../services/dish.service';
import { FormControl, FormGroup } from '@angular/forms';
import { timer } from 'rxjs';

@Component({
  selector: 'app-dish-overview',
  templateUrl: './dish-overview.component.html'
})
export class DishOverviewComponent implements OnInit, AfterViewInit {
  private _dishes: Dish[] = [];
  private lastSearch: string = '';

  @ViewChild('search')
  search: ElementRef | undefined;

  form = new FormGroup({
    search: new FormControl<string>('')
  });

  constructor(private dishService: DishService) {}

  async ngOnInit(): Promise<void> {
    this._dishes = await this.dishService.get(50);
  }

  ngAfterViewInit(): void {
    this.search?.nativeElement.focus();
  }

  dishesForDisplay(): Dish[] {
    return this._dishes;
  }

  async onSearchInput(): Promise<void> {
    const searchterm = this.form.controls.search.value?.trim().toLowerCase();

    if (
      searchterm != undefined &&
      (searchterm!.length >= 2 || (searchterm == '' && this.lastSearch != ''))
    ) {
      this.lastSearch = searchterm;
      timer(800).subscribe(() => {
        if (searchterm == this.lastSearch) this.submitSearch(searchterm);
      });
    }
  }

  async onClickSearch() {
    const searchterm = this.form.controls.search.value?.trim().toLowerCase();

    if (searchterm != undefined) this.submitSearch(searchterm);
  }

  async submitSearch(searchterm: string): Promise<void> {
    this._dishes = await this.dishService.get(undefined, searchterm);
  }
}

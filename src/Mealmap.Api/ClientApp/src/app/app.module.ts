import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MealScheduleComponent } from './meal-schedule/meal-schedule.component';
import { DishOverviewComponent } from './dish-overview/dish-overview.component';
import { MealCardComponent } from './meal-card/meal-card.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    MealScheduleComponent,
    DishOverviewComponent,
    MealCardComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: MealScheduleComponent, pathMatch: 'full' },
      { path: 'meals', component: MealScheduleComponent },
      { path: 'dishes', component: DishOverviewComponent },
    ]),
    NgbModule
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

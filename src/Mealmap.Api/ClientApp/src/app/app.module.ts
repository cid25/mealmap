import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import routes from './routes';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MealScheduleComponent } from './meal-schedule/meal-schedule.component';
import { DishOverviewComponent } from './dish-overview/dish-overview.component';
import { MealCardComponent } from './meal-card/meal-card.component';
import { MealEditorComponent } from './meal-editor/meal-editor.component';
import { DishPickerComponent } from './dish-picker/dish-picker.component';
import { DishCardComponent } from './dish-card/dish-card.component';
import { DishEditorComponent } from './dish-editor/dish-editor.component';
import { DishViewerComponent } from './dish-viewer/dish-viewer.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    MealScheduleComponent,
    MealCardComponent,
    MealEditorComponent,
    DishPickerComponent,
    DishOverviewComponent,
    DishCardComponent,
    DishEditorComponent,
    DishViewerComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    ReactiveFormsModule,
    RouterModule.forRoot(routes),
    NgbModule
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}

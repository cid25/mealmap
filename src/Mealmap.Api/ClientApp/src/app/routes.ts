import { Routes } from '@angular/router';
import { DishOverviewComponent } from './dish-overview/dish-overview.component';
import { MealScheduleComponent } from './meal-schedule/meal-schedule.component';
import { MealEditorComponent } from './meal-editor/meal-editor.component';

const routes: Routes = [
  { path: 'meals/:date', component: MealEditorComponent },
  { path: 'meals', component: MealScheduleComponent },
  { path: 'dishes', component: DishOverviewComponent },
  { path: '**', redirectTo: 'meals' }
];

export default routes;

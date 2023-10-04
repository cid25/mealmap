import { Routes } from '@angular/router';
import { MealScheduleComponent } from './meal-schedule/meal-schedule.component';
import { MealEditorComponent } from './meal-editor/meal-editor.component';
import { DishOverviewComponent } from './dish-overview/dish-overview.component';
import { DishEditorComponent } from './dish-editor/dish-editor.component';
import { DishViewerComponent } from './dish-viewer/dish-viewer.component';

const routes: Routes = [
  { path: 'meals/:date', component: MealEditorComponent, title: 'Meal Details' },
  { path: 'meals', component: MealScheduleComponent, title: 'Meal Schedule' },
  { path: 'dishes/new', component: DishEditorComponent, title: 'Dish Details' },
  { path: 'dishes/:id/edit', component: DishEditorComponent, title: 'Dish Details' },
  { path: 'dishes/:id', component: DishViewerComponent, title: 'Dish Details' },
  { path: 'dishes', component: DishOverviewComponent, title: 'Dishes' },
  { path: '**', redirectTo: 'meals' }
];

export default routes;

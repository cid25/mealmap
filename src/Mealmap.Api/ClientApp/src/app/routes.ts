import { Routes } from '@angular/router';
import { MealScheduleComponent } from './meal-schedule/meal-schedule.component';
import { MealEditorComponent } from './meal-editor/meal-editor.component';
import { MealViewerComponent } from './meal-viewer/meal-viewer.component';
import { DishOverviewComponent } from './dish-overview/dish-overview.component';
import { DishEditorComponent } from './dish-editor/dish-editor.component';
import { DishViewerComponent } from './dish-viewer/dish-viewer.component';

const routes: Routes = [
  { path: 'meals/:date/edit', component: MealEditorComponent, title: 'Meal Details' },
  { path: 'meals/:date', component: MealViewerComponent, title: 'Meal Details' },
  { path: 'meals', component: MealScheduleComponent, title: 'Meal Schedule' },
  { path: 'dishes/new', component: DishEditorComponent, title: 'Dish Editor' },
  { path: 'dishes/:id/edit', component: DishEditorComponent, title: 'Dish Editor' },
  { path: 'dishes/:id', component: DishViewerComponent, title: 'Dish Viewer' },
  { path: 'dishes', component: DishOverviewComponent, title: 'Dishes' },
  { path: '**', redirectTo: 'meals' }
];

export default routes;

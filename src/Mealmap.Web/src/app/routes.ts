import { Routes } from '@angular/router';
import { MealScheduleComponent } from './components/meal-schedule/meal-schedule.component';
import { MealEditorComponent } from './components/meal-editor/meal-editor.component';
import { MealViewerComponent } from './components/meal-viewer/meal-viewer.component';
import { DishOverviewComponent } from './components/dish-overview/dish-overview.component';
import { DishEditorComponent } from './components/dish-editor/dish-editor.component';
import { DishViewerComponent } from './components/dish-viewer/dish-viewer.component';

const routes: Routes = [
  { path: 'meals/current', component: MealScheduleComponent, title: 'Meal Schedule' },
  { path: 'meals/:date/edit', component: MealEditorComponent, title: 'Meal Details' },
  { path: 'meals/:date', component: MealViewerComponent, title: 'Meal Details' },
  { path: 'meals', component: MealScheduleComponent, title: 'Meal Schedule' },
  { path: 'dishes/new', component: DishEditorComponent, title: 'Dish Editor' },
  { path: 'dishes/:id/edit', component: DishEditorComponent, title: 'Dish Editor' },
  { path: 'dishes/:id', component: DishViewerComponent, title: 'Dish Viewer' },
  { path: 'dishes', component: DishOverviewComponent, title: 'Dishes' },
  { path: '**', redirectTo: 'meals/current' }
];

export default routes;

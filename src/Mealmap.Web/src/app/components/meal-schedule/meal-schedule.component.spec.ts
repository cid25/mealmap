import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MealScheduleComponent } from './meal-schedule.component';
import { provideRouter } from '@angular/router';
import { MealService } from 'src/app/services/meal.service';

describe('MealScheduleComponent', () => {
  let component: MealScheduleComponent;
  let fixture: ComponentFixture<MealScheduleComponent>;
  let mealServiceSpy: jasmine.SpyObj<MealService>;

  beforeEach(async () => {
    mealServiceSpy = jasmine.createSpyObj('MealService', ['getMealsFor', 'deleteMeal']);

    await TestBed.configureTestingModule({
      declarations: [MealScheduleComponent],
      providers: [provideRouter([]), { provide: MealService, useValue: mealServiceSpy }]
    }).compileComponents();

    fixture = TestBed.createComponent(MealScheduleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

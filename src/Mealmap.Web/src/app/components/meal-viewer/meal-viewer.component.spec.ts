import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MealViewerComponent } from './meal-viewer.component';
import { MealService } from 'src/app/services/meal.service';
import { Meal } from 'src/app/domain/meal';
import { DateTime } from 'luxon';
import { provideRouter } from '@angular/router';

describe('MealViewerComponent', () => {
  let component: MealViewerComponent;
  let fixture: ComponentFixture<MealViewerComponent>;
  let mealServiceSpy: jasmine.SpyObj<MealService>;
  let locationSpy: jasmine.SpyObj<Location>;

  beforeEach(async () => {
    mealServiceSpy = jasmine.createSpyObj('MealService', ['getMealFor']);
    locationSpy = jasmine.createSpyObj('Location', ['back']);

    await TestBed.configureTestingModule({
      declarations: [MealViewerComponent],
      providers: [
        provideRouter([]),
        { provide: MealService, useValue: mealServiceSpy },
        { provide: Location, useValue: locationSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MealViewerComponent);
    component = fixture.componentInstance;
    component.meal = new Meal(DateTime.now().toJSDate());
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

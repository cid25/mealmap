import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MealEditorComponent } from './meal-editor.component';
import { provideRouter } from '@angular/router';
import { DateTime } from 'luxon';
import { MealService } from 'src/app/services/meal.service';
import { Meal } from 'src/app/domain/meal';

describe('MealEditorComponent', () => {
  let component: MealEditorComponent;
  let fixture: ComponentFixture<MealEditorComponent>;
  let locationSpy: jasmine.SpyObj<Location>;

  beforeEach(async () => {
    const date = DateTime.now().toJSDate();
    locationSpy = jasmine.createSpyObj('Location', ['back']);
    const mealServiceStub = {
      getMealFor: (date: Date) => {
        return new Meal(date);
      },
      saveMeal: () => undefined
    };

    await TestBed.configureTestingModule({
      declarations: [MealEditorComponent],
      providers: [
        provideRouter([]),
        { provide: Location, useValue: locationSpy },
        { provide: MealService, useValue: mealServiceStub as unknown as MealService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MealEditorComponent);
    component = fixture.componentInstance;
    component.diningDate = date;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

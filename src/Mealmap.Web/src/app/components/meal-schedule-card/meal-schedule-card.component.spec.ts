import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MealScheduleCardComponent } from './meal-schedule-card.component';
import { Meal } from 'src/app/domain/meal';
import { DateTime } from 'luxon';

describe('MealScheduleCardComponent', () => {
  let component: MealScheduleCardComponent;
  let fixture: ComponentFixture<MealScheduleCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [MealScheduleCardComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(MealScheduleCardComponent);
    component = fixture.componentInstance;
    component.meal = new Meal(DateTime.now().toJSDate());
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

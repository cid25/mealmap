import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MealScheduleCardComponent } from './meal-schedule-card.component';

describe('MealScheduleCardComponent', () => {
  let component: MealScheduleCardComponent;
  let fixture: ComponentFixture<MealScheduleCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [MealScheduleCardComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(MealScheduleCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

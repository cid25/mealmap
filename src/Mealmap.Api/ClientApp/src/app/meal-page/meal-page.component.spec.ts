import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MealPageComponent } from './meal-page.component';

describe('MealPageComponent', () => {
  let component: MealPageComponent;
  let fixture: ComponentFixture<MealPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MealPageComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MealPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

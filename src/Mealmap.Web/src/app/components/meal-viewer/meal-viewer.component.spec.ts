import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MealViewerComponent } from './meal-viewer.component';

describe('MealViewerComponent', () => {
  let component: MealViewerComponent;
  let fixture: ComponentFixture<MealViewerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MealViewerComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MealViewerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DishOverviewComponent } from './dish-overview.component';

describe('DishOverviewComponent', () => {
  let component: DishOverviewComponent;
  let fixture: ComponentFixture<DishOverviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DishOverviewComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DishOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

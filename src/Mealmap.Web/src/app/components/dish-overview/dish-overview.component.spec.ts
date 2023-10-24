import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DishOverviewComponent } from './dish-overview.component';
import { DishService } from 'src/app/services/dish.service';

describe('DishOverviewComponent', () => {
  let component: DishOverviewComponent;
  let fixture: ComponentFixture<DishOverviewComponent>;
  let dishServiceSpy: jasmine.SpyObj<DishService>;

  beforeEach(async () => {
    dishServiceSpy = jasmine.createSpyObj('DishService', ['get']);

    await TestBed.configureTestingModule({
      declarations: [DishOverviewComponent],
      providers: [{ provide: DishService, useValue: dishServiceSpy }]
    }).compileComponents();

    fixture = TestBed.createComponent(DishOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

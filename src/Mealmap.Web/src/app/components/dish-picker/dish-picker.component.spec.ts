import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DishPickerComponent } from './dish-picker.component';
import { DishService } from 'src/app/services/dish.service';

describe('DishPickerComponent', () => {
  let component: DishPickerComponent;
  let fixture: ComponentFixture<DishPickerComponent>;
  let dishServiceSpy: jasmine.SpyObj<DishService>;

  beforeEach(async () => {
    dishServiceSpy = jasmine.createSpyObj('DishService', ['get', 'getById']);

    await TestBed.configureTestingModule({
      declarations: [DishPickerComponent],
      providers: [{ provide: DishService, useValue: dishServiceSpy }]
    }).compileComponents();

    fixture = TestBed.createComponent(DishPickerComponent);
    component = fixture.componentInstance;
    component.index = 1;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

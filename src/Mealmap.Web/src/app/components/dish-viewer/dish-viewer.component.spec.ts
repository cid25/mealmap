import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DishViewerComponent } from './dish-viewer.component';
import { provideRouter } from '@angular/router';
import { DishService } from 'src/app/services/dish.service';
import { Dish } from 'src/app/domain/dish';

describe('DishViewerComponent', () => {
  let component: DishViewerComponent;
  let fixture: ComponentFixture<DishViewerComponent>;
  let locationSpy: jasmine.SpyObj<Location>;
  let dishServiceSpy: jasmine.SpyObj<DishService>;

  beforeEach(async () => {
    locationSpy = jasmine.createSpyObj('Location', ['back']);
    dishServiceSpy = jasmine.createSpyObj('DishService', ['getById']);

    await TestBed.configureTestingModule({
      declarations: [DishViewerComponent],
      providers: [
        provideRouter([]),
        { provide: Location, useValue: locationSpy },
        { provide: DishService, useValue: dishServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(DishViewerComponent);
    component = fixture.componentInstance;
    const dish = new Dish();
    dish.id = crypto.randomUUID();
    component.dish = dish;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should not have ingredients', () => {
    const result = component.hasIngredients();
    expect(result).toBeFalse();
  });
});

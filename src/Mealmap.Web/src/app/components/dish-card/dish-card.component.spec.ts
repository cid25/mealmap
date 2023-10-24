import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DishCardComponent } from './dish-card.component';
import { Dish } from 'src/app/domain/dish';

describe('DishCardComponent', () => {
  let component: DishCardComponent;
  let fixture: ComponentFixture<DishCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DishCardComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(DishCardComponent);

    component = fixture.componentInstance;
    const dish = new Dish();
    dish.id = crypto.randomUUID();
    dish.name = 'Dummy';
    component.dish = dish;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeDefined();
  });
});

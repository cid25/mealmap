import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { MealService } from './meal.service';

describe('MealsService', () => {
  let service: MealService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [MealService]
    });
    service = TestBed.inject(MealService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

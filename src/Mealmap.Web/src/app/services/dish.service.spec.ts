import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { DishService } from './dish.service';

describe('DishesService', () => {
  let service: DishService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [DishService]
    });
    service = TestBed.inject(DishService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

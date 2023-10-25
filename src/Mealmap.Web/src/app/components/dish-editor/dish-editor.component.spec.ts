import { TestBed } from '@angular/core/testing';
import { DishEditorComponent } from './dish-editor.component';
import { provideRouter } from '@angular/router';
import { DishService } from 'src/app/services/dish.service';
import { RouterTestingHarness } from '@angular/router/testing';

describe('DishEditorComponent', () => {
  let harness: RouterTestingHarness;
  let component: DishEditorComponent;
  let locationSpy: jasmine.SpyObj<Location>;
  let dishServiceSpy: jasmine.SpyObj<DishService>;

  beforeEach(async () => {
    locationSpy = jasmine.createSpyObj('Location', ['back']);
    dishServiceSpy = jasmine.createSpyObj('DishService', ['getById', 'save', 'delete', 'urlFor']);

    await TestBed.configureTestingModule({
      declarations: [DishEditorComponent],
      providers: [
        provideRouter([{ path: 'new', component: DishEditorComponent }]),
        { provide: Location, useValue: locationSpy },
        { provide: DishService, useValue: dishServiceSpy }
      ]
    })
      .compileComponents()
      .then(async () => {
        harness = await RouterTestingHarness.create();
        component = await harness.navigateByUrl('new', DishEditorComponent);
      });

    harness.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

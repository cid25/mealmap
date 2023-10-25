import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { By } from '@angular/platform-browser';
import { Dish } from 'src/app/domain/dish';
import { DishPickedEvent } from 'src/app/events/dish-picked.event';
import { DishService } from 'src/app/services/dish.service';
import { DishPickerComponent } from './dish-picker.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('DishPickerComponent', () => {
  let testhost: TestHostComponent;
  let fixture: ComponentFixture<TestHostComponent>;
  let testdish: Dish;
  // eslint-disable-next-line
  let dishServiceMock: any;

  beforeEach(async () => {
    testdish = new Dish();
    testdish.id = crypto.randomUUID();
    dishServiceMock = {
      get: async () => {
        return [testdish];
      },
      save: async () => {
        return testdish;
      }
    };

    await TestBed.configureTestingModule({
      declarations: [DishPickerComponent, TestHostComponent],
      imports: [ReactiveFormsModule],
      providers: [{ provide: DishService, useValue: dishServiceMock }],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(TestHostComponent);
    testhost = fixture.componentInstance;
  });

  it('should allow selecting a dish via button', () => {
    fixture.autoDetectChanges();
    fixture.whenStable().then(() => {
      const selectbutton = fixture.debugElement.query(By.css('#btnselect-0'));
      selectbutton.triggerEventHandler('click');

      expect(testhost.dishPickedEvents.length).toBe(1);
      expect(testhost.dishPickedEvents[0].index).toBe(0);
      expect(testhost.dishPickedEvents[0].dish).toEqual(testdish);
    });
  });
});

@Component({
  template: `<app-dish-picker> [index]="0" (picked)="onPicked($event)" </app-dish-picker>`
})
class TestHostComponent {
  dishPickedEvents: DishPickedEvent[] = [];
  onPicked(event: DishPickedEvent): void {
    this.dishPickedEvents.push(event);
  }
}

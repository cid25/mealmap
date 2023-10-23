import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MealEditorComponent } from './meal-editor.component';

describe('MealEditorComponent', () => {
  let component: MealEditorComponent;
  let fixture: ComponentFixture<MealEditorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MealEditorComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MealEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

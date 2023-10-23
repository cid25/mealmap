import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MealEditorCardComponent } from './meal-editor-card.component';

describe('MealEditorCardComponent', () => {
  let component: MealEditorCardComponent;
  let fixture: ComponentFixture<MealEditorCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [MealEditorCardComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(MealEditorCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

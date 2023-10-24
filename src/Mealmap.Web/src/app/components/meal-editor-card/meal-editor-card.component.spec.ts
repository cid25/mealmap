import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MealEditorCardComponent } from './meal-editor-card.component';
import { Course } from 'src/app/domain/course';

describe('MealEditorCardComponent', () => {
  let component: MealEditorCardComponent;
  let fixture: ComponentFixture<MealEditorCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [MealEditorCardComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(MealEditorCardComponent);
    component = fixture.componentInstance;
    component.course = new Course(1, crypto.randomUUID(), 2);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

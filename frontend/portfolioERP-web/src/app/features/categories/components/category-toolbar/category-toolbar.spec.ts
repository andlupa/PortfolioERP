import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CategoryToolbar } from './category-toolbar';

describe('CategoryToolbar', () => {
  let component: CategoryToolbar;
  let fixture: ComponentFixture<CategoryToolbar>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CategoryToolbar],
    }).compileComponents();

    fixture = TestBed.createComponent(CategoryToolbar);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

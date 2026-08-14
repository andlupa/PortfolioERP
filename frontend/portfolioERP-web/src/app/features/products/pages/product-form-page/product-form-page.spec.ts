import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductFormPage } from './product-form-page';

describe('ProductFormPage', () => {
  let component: ProductFormPage;
  let fixture: ComponentFixture<ProductFormPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProductFormPage],
    }).compileComponents();

    fixture = TestBed.createComponent(ProductFormPage);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

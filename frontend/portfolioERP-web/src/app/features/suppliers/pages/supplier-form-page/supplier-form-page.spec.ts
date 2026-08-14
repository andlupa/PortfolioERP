import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SupplierFormPage } from './supplier-form-page';

describe('SupplierFormPage', () => {
  let component: SupplierFormPage;
  let fixture: ComponentFixture<SupplierFormPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SupplierFormPage],
    }).compileComponents();

    fixture = TestBed.createComponent(SupplierFormPage);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

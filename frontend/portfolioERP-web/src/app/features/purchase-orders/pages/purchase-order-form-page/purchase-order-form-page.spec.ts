import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PurchaseOrderFormPage } from './purchase-order-form-page';

describe('PurchaseOrderFormPage', () => {
  let component: PurchaseOrderFormPage;
  let fixture: ComponentFixture<PurchaseOrderFormPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PurchaseOrderFormPage],
    }).compileComponents();

    fixture = TestBed.createComponent(PurchaseOrderFormPage);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PurchaseOrderDetailPage } from './purchase-order-detail-page';

describe('PurchaseOrderDetailPage', () => {
  let component: PurchaseOrderDetailPage;
  let fixture: ComponentFixture<PurchaseOrderDetailPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PurchaseOrderDetailPage],
    }).compileComponents();

    fixture = TestBed.createComponent(PurchaseOrderDetailPage);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

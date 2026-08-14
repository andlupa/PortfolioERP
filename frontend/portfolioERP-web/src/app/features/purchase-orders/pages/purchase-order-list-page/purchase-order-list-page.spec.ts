import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PurchaseOrderListPage } from './purchase-order-list-page';

describe('PurchaseOrderListPage', () => {
  let component: PurchaseOrderListPage;
  let fixture: ComponentFixture<PurchaseOrderListPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PurchaseOrderListPage],
    }).compileComponents();

    fixture = TestBed.createComponent(PurchaseOrderListPage);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

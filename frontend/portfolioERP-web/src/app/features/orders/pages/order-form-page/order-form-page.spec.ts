import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderFormPage } from './order-form-page';

describe('OrderFormPage', () => {
  let component: OrderFormPage;
  let fixture: ComponentFixture<OrderFormPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OrderFormPage],
    }).compileComponents();

    fixture = TestBed.createComponent(OrderFormPage);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

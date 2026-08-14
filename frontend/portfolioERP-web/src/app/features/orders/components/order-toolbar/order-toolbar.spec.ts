import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderToolbar } from './order-toolbar';

describe('OrderToolbar', () => {
  let component: OrderToolbar;
  let fixture: ComponentFixture<OrderToolbar>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OrderToolbar],
    }).compileComponents();

    fixture = TestBed.createComponent(OrderToolbar);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

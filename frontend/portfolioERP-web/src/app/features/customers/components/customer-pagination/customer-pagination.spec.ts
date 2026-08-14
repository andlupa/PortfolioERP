import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerPagination } from './customer-pagination';

describe('CustomerPagination', () => {
  let component: CustomerPagination;
  let fixture: ComponentFixture<CustomerPagination>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CustomerPagination],
    }).compileComponents();

    fixture = TestBed.createComponent(CustomerPagination);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

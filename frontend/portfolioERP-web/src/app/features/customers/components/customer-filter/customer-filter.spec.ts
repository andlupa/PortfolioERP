import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerFilter } from './customer-filter';

describe('CustomerFilter', () => {
  let component: CustomerFilter;
  let fixture: ComponentFixture<CustomerFilter>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CustomerFilter],
    }).compileComponents();

    fixture = TestBed.createComponent(CustomerFilter);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerToolbar } from './customer-toolbar';

describe('CustomerToolbar', () => {
  let component: CustomerToolbar;
  let fixture: ComponentFixture<CustomerToolbar>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CustomerToolbar],
    }).compileComponents();

    fixture = TestBed.createComponent(CustomerToolbar);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SupplierListPage } from './supplier-list-page';

describe('SupplierListPage', () => {
  let component: SupplierListPage;
  let fixture: ComponentFixture<SupplierListPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SupplierListPage],
    }).compileComponents();

    fixture = TestBed.createComponent(SupplierListPage);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

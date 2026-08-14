import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AppToast } from './app-toast';

describe('AppToast', () => {
  let component: AppToast;
  let fixture: ComponentFixture<AppToast>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppToast],
    }).compileComponents();

    fixture = TestBed.createComponent(AppToast);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

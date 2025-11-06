import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SsoCallback } from './sso-callback';

describe('SsoCallback', () => {
  let component: SsoCallback;
  let fixture: ComponentFixture<SsoCallback>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SsoCallback]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SsoCallback);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

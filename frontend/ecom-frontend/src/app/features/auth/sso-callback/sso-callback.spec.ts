import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { SsoCallback } from './sso-callback';
import { provideRouter } from '@angular/router';

describe('SsoCallback', () => {
  let component: SsoCallback;
  let fixture: ComponentFixture<SsoCallback>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SsoCallback, HttpClientTestingModule],
      providers: [provideRouter([])]
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

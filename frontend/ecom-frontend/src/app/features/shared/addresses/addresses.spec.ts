import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { Addresses } from './addresses';

describe('Addresses', () => {
  let component: Addresses;
  let fixture: ComponentFixture<Addresses>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Addresses, HttpClientTestingModule]
    })
      .compileComponents();

    fixture = TestBed.createComponent(Addresses);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

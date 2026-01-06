import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { MyProfileComponent } from './my-profile';

describe('MyProfile', () => {
  let component: MyProfileComponent;
  let fixture: ComponentFixture<MyProfileComponent>;
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MyProfileComponent, HttpClientTestingModule],
      providers: [provideRouter([])]
    })
      .compileComponents();

    fixture = TestBed.createComponent(MyProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

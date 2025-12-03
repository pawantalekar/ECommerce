import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { WriteReviewPageComponent } from './write-review-page';
import { provideRouter } from '@angular/router';

describe('WriteReviewPageComponent', () => {
  let component: WriteReviewPageComponent;
  let fixture: ComponentFixture<WriteReviewPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WriteReviewPageComponent, HttpClientTestingModule],
      providers: [provideRouter([])]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WriteReviewPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

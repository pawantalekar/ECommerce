import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { MessageService } from 'primeng/api';
import { ProfileDetails } from './profile-details';

describe('ProfileDetails', () => {
  let component: ProfileDetails;
  let fixture: ComponentFixture<ProfileDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProfileDetails, HttpClientTestingModule],
      providers: [MessageService]
    })
      .compileComponents();

    fixture = TestBed.createComponent(ProfileDetails);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

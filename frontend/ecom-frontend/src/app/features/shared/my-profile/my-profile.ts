import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { MyProfileService, UpdateMyProfileCommand, UserProfileDto } from '../services/my-profile-service';
import { AuthService } from '../../../core/services/auth-service';
import { AuthRoutingModule } from "../../auth/auth-routing-module";
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-my-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, AuthRoutingModule, ToastModule],
  templateUrl: './my-profile.html',
  styleUrls: ['./my-profile.css'],
  providers: [MessageService]
})
export class MyProfileComponent implements OnInit, OnDestroy  {
  private myProfileService = inject(MyProfileService);
  private authService = inject(AuthService);
  private profileSub?: Subscription;

  userProfile?: UserProfileDto;
  loading = true;
  error?: string;
  
  ngOnInit() {
    this.loadProfile();
    this.profileSub = this.myProfileService.profileUpdated$.subscribe(() => {
      this.loadProfile();
    });
  }

  ngOnDestroy() {
    this.profileSub?.unsubscribe();
  }
  constructor() {
    this.loadProfile();
  }

  loadProfile() {
    this.loading = true;
    this.error = undefined;

    this.myProfileService.getMyProfile().subscribe({
      next: (response) => {
        this.userProfile = response.result;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load profile. Please try again.';
        this.loading = false;
      }
    });
  }
  logout() {
    this.authService.logout();
  }
}
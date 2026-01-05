import { Component, inject, OnInit } from '@angular/core';
import { MyProfileService, UpdateMyProfileCommand, UserProfileDto } from '../services/my-profile-service';
import { AuthService } from '../../../core/services/auth-service';
import { MessageService } from 'primeng/api';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-profile-details',
  imports: [FormsModule, CommonModule],
  templateUrl: './profile-details.html',
  styleUrl: './profile-details.css',
})
export class ProfileDetails implements OnInit {
  private myProfileService = inject(MyProfileService);
  private authService = inject(AuthService);
  private messageService = inject(MessageService);

  userProfile?: UserProfileDto;
  loading = true;
  error?: string;

  updateCommand: UpdateMyProfileCommand = {
    firstName: '',
    lastName: '',
    gender: '',
    mobileNumber: ''
  };

  private originalCommand: UpdateMyProfileCommand = { ...this.updateCommand };
  ngOnInit() {
    this.loading = true;
    this.error = undefined;
    this.myProfileService.getMyProfile().subscribe({
      next: (profileResponse) => {
        this.userProfile = profileResponse.result;
        this.resetUpdateCommand();
        this.loading = false;
      },
      error: (err) => {
        this.error = err.error?.message || 'Failed to load profile.';
        this.loading = false;
      }
    });
  }
  private resetUpdateCommand() {
    if (this.userProfile) {
      this.updateCommand = {
        firstName: this.userProfile.firstName || '',
        lastName: this.userProfile.lastName || '',
        gender: this.userProfile.gender || '',
        mobileNumber: this.userProfile.mobileNumber || ''
      };
      this.originalCommand = { ...this.updateCommand };
    }
  }

  hasChanges(): boolean {
    if (!this.userProfile) return false;

    return (
      this.updateCommand.firstName.trim() !== (this.userProfile.firstName || '').trim() ||
      this.updateCommand.lastName.trim() !== (this.userProfile.lastName || '').trim() ||
      this.updateCommand.gender !== (this.userProfile.gender || '') ||
      this.updateCommand.mobileNumber.trim() !== (this.userProfile.mobileNumber || '').trim()
    );
  }

  updateProfile(form: NgForm) {
    if (form.invalid || !this.hasChanges()) {
      return;
    }

    this.loading = true;
    this.error = undefined;

    this.myProfileService.updateMyProfile(this.updateCommand).subscribe({
      next: () => {
        this.myProfileService.getMyProfile().subscribe({
          next: (profileResponse) => {
            this.userProfile = profileResponse.result;
            this.resetUpdateCommand();
            this.loading = false;
            this.messageService.add({ severity: 'success', detail: 'Profile updated successfully!' });
            this.myProfileService.notifyProfileUpdated();
          },
          error: () => {
            this.error = 'Failed to refresh profile after update.';
            this.loading = false;
          }
        });
      },
      error: (err) => {
        this.error = err.error?.message || 'Failed to update profile. Please try again.';
        this.loading = false;
      }
    });
  }
  // Allow only letters and spaces for names
  onlyLetters(event: KeyboardEvent): boolean {
    const charCode = event.which ? event.which : event.keyCode;
    if ((charCode >= 65 && charCode <= 90) || (charCode >= 97 && charCode <= 122) || charCode === 32) {
      return true;
    }
    event.preventDefault();
    return false;
  }

  // Prevent pasting special characters in names
  sanitizeName(event: Event): void {
    const input = event.target as HTMLInputElement;
    input.value = input.value.replace(/[^A-Za-z\s]/g, '');
    this.updateCommand[input.name as 'firstName' | 'lastName'] = input.value;
  }

  // Allow only numbers for mobile
  onlyNumbers(event: KeyboardEvent): boolean {
    const charCode = event.which ? event.which : event.keyCode;
    if (charCode >= 48 && charCode <= 57) {
      return true;
    }
    event.preventDefault();
    return false;
  }

  // Enforce exactly 10 digits max for mobile
  limitTo10Digits(event: Event): void {
    const input = event.target as HTMLInputElement;
    input.value = input.value.replace(/[^0-9]/g, '').slice(0, 10);
    this.updateCommand.mobileNumber = input.value;
  }
}

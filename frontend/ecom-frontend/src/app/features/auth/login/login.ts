import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators, NgForm } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth-service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.html',
  styleUrls: ['./login.css']
})
export class Login implements OnInit {
  form!: FormGroup;
  loading = false;
  error: string | null = null;

  constructor(private fb: FormBuilder, private auth: AuthService) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      username: ['', [Validators.minLength(0)]],
      password: ['', [Validators.minLength(0)]],
      remember: [false]
    });
  }

  get f() {
    return this.form.controls;
  }

  // Sign in with Google
  loginWithGoogle(): void {
    this.auth.loginWithGoogle();
  }

  // UI-only submit (placeholder)
  onSubmit(): void {
    this.error = null;
    this.form.markAllAsTouched();
    this.error = 'Username/password login not implemented in this demo.';
  }

  // Placeholder for forgot password
  onForgot(): void { }
}

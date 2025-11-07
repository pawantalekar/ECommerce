import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable, BehaviorSubject, tap } from 'rxjs';

export interface User {
  accessToken: string;
  refreshToken?: string;
  expires?: string;
  // add other fields returned by backend (name, email) if needed
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private userKey = 'currentUser';
  private userSubject = new BehaviorSubject<User | null>(this.getUser());
  user$ = this.userSubject.asObservable();

  constructor(private http: HttpClient) { }

  loginWithGoogle(): void {
    const redirectUri = `${window.location.origin}/auth/callback`;
    window.location.href = `${environment.apiBaseUrl}/Auth/google-login?redirect_uri=${encodeURIComponent(redirectUri)}`;
  }

  completeSso(code: string): Observable<User> {
    return this.http.get<User>(`${environment.apiBaseUrl}/Auth/google-response?code=${encodeURIComponent(code)}`)
      .pipe(
        tap(user => this.setUser(user))
      );
  }

  setUser(user: User): void {
    localStorage.setItem(this.userKey, JSON.stringify(user));
    this.userSubject.next(user);
    history.replaceState(null, '', location.origin + '/home');
  }

  getUser(): User | null {
    const raw = localStorage.getItem(this.userKey);
    return raw ? JSON.parse(raw) : null;
  }

  logout(): void {
    localStorage.removeItem(this.userKey);
    this.userSubject.next(null);
  }

  isLoggedIn(): boolean {
    return !!this.getUser()?.accessToken;
  }
}

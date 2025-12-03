import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable, BehaviorSubject, tap, map } from 'rxjs';
import { Router } from '@angular/router';

export interface User {
  accessToken: string;
  refreshToken?: string;
  expires?: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private userKey = 'currentUser';
  private userSubject = new BehaviorSubject<User | null>(this.getUser());
  user$ = this.userSubject.asObservable();
  private roleSubject = new BehaviorSubject<string>('User');
  role$ = this.roleSubject.asObservable();
  constructor(private http: HttpClient, private router: Router) { }

  loginWithGoogle(): void {
    const redirectUri = `${window.location.origin}/auth/callback`;
    window.location.href = `${environment.apiBaseUrl}/Auth/google-login?redirect_uri=${encodeURIComponent(redirectUri)}`;
  }

  completeSso(code: string): Observable<User> {
    return this.http.get<User>(`${environment.apiBaseUrl}/Auth/google-response?code=${encodeURIComponent(code)}`)
      .pipe(
        tap(user => {
          this.setUser(user);
          this.loadUserRole();   
        })
      );
  }
  loadUserRole(): void {
  this.http.get<{ role: string }>(`${environment.apiBaseUrl}/auth/me`)
    .subscribe({
      next: (res) => {
        localStorage.setItem('userRole', res.role);
        this.roleSubject.next(res.role);  
      },
      error: () => {
        localStorage.setItem('userRole', 'User');
        this.roleSubject.next('User');
      }
    });
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


  private getCleanRefreshToken(): string | null {
    const user = this.getUser();
    return user?.refreshToken?.replace(/ /g, '+') || null;
  }

  logout(): void {
    const token = this.getCleanRefreshToken();
    if (token) {
      this.http.post(`${environment.apiBaseUrl}/Auth/logout`, { refreshToken: token })
        .subscribe({
          next: () => this.finalLogout(),
          error: () => this.finalLogout()
        });
    } else {
      this.finalLogout();
    }
  }

  private finalLogout(): void {
    localStorage.removeItem(this.userKey);
    this.userSubject.next(null);
    setTimeout(() => this.router.navigate(['/auth/login']), 200);
  }

  isLoggedIn(): boolean {
    return !!this.getUser()?.accessToken;
  }

  getRole(): string {
  return this.roleSubject.value;   
}

  refreshToken(oldToken: string): Observable<string> {
    return this.http.post<any>(`${environment.apiBaseUrl}/auth/token/refresh`, {
      refreshToken: oldToken
    }).pipe(
      tap(res => {
        this.setUser({
          accessToken: res.accessToken,
          refreshToken: res.refreshToken,
          expires: res.expires
        });
      }),
      map(res => res.accessToken)
    );
  }
}
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable, BehaviorSubject } from 'rxjs';

export interface User {
  accessToken: string;
  refreshToken: string;
  expires: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private userSubject = new BehaviorSubject<User | null>(null);
  user$ = this.userSubject.asObservable();

  constructor(private http: HttpClient) { }

  loginWithGoogle() {
    window.location.href = `${environment.apiBaseUrl}/Auth/google-login`;
  }

  handleGoogleResponse(code: string): Observable<User> {
    return this.http.get<User>(`${environment.apiBaseUrl}/Auth/google-response?code=${code}`);
  }

  setUser(user: User) {
    this.userSubject.next(user);
    localStorage.setItem('user', JSON.stringify(user));
  }

  getUser(): User | null {
    const stored = localStorage.getItem('user');
    return stored ? JSON.parse(stored) : null;
  }

  logout() {
    this.userSubject.next(null);
    localStorage.removeItem('user');
  }
  isLoggedIn(): boolean {
    return !!this.getUser();
  }
  completeSso(code: string): Observable<any> {
    return this.http.get(`${environment.apiBaseUrl}/Auth/google-response?code=${code}`);
  }

}

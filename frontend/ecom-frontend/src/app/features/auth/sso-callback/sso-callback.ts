import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth-service';

@Component({
  selector: 'app-sso-callback',
  standalone: true,
  template: `<p>Signing in...</p>`
})
export class SsoCallback implements OnInit {
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private auth: AuthService
  ) { }

  ngOnInit(): void {
    this.route.queryParamMap.subscribe(p => {
      const token = p.get('accessToken');
      if (token) {
        this.auth.setUser({
          accessToken: token,
          refreshToken: p.get('refreshToken') ?? undefined,
          expires: p.get('expires') ?? undefined
        });
        this.router.navigate(['/home'], { replaceUrl: true });
      } else {
        this.router.navigate(['/auth/login'], { replaceUrl: true });
      }
    });
  }
}
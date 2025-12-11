import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth-service';

@Component({
  selector: 'app-sso-callback',
  standalone: true,
  template: '<p>Signing in...</p>'
})
export class SsoCallback implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private auth = inject(AuthService);


  ngOnInit(): void {
    this.route.queryParamMap.subscribe(p => {
      const token = p.get('accessToken');
      if (token) {
        this.auth.setUser({
          accessToken: token,
          refreshToken: p.get('refreshToken') ?? undefined,
          expires: p.get('expires') ?? undefined
        });
        this.auth.loadUserRole();
        this.router.navigate(['/home'], { replaceUrl: true });
      } else {
        this.router.navigate(['/auth/login']);
      }
    });
  }
}
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth-service';

@Component({
  selector: 'app-sso-callback',
  standalone: true,
  template: `<p>Signing in...</p>`
})
export class SsoCallback implements OnInit {
  constructor(private route: ActivatedRoute, private router: Router, private auth: AuthService) { }

  ngOnInit() {
    const code = this.route.snapshot.queryParamMap.get('code');
    if (code) {
      this.auth.completeSso(code).subscribe({
        next: () => this.router.navigate(['/dashboard']),
        error: (err: any) => console.error(err)
      });
    }
  }
}

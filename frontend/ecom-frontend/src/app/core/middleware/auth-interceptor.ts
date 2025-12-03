import { HttpInterceptorFn } from "@angular/common/http";
import { AuthService } from "../services/auth-service";
import { inject } from "@angular/core";

export const AuthInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const user = auth.getUser();

  console.log("INTERCEPTOR TOKEN:", user?.accessToken);

  if (req.url.includes('/auth/logout')) {
    return next(req);   
  }
  if (user?.accessToken) {
    req = req.clone({
      setHeaders: { Authorization: `Bearer ${user.accessToken}` }
    });
  }

  return next(req);
};

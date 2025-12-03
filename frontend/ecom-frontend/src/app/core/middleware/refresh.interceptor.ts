import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth-service';
import { BehaviorSubject, throwError } from 'rxjs';
import { catchError, filter, switchMap, take } from 'rxjs/operators';

let isRefreshing = false;
const refreshSubject = new BehaviorSubject<string | null>(null);

export const RefreshInterceptor: HttpInterceptorFn = (req, next) => {
    const auth = inject(AuthService);

    return next(req).pipe(
        catchError(error => {
            if (error.status === 401 && auth.getUser()?.refreshToken && !req.url.includes('/auth/')) {
                const refreshToken = auth.getUser()!.refreshToken!;

                if (!isRefreshing) {
                    isRefreshing = true;
                    refreshSubject.next(null);

                    return auth.refreshToken(refreshToken).pipe(
                        switchMap(newAccessToken => {
                            isRefreshing = false;
                            refreshSubject.next(newAccessToken);
                            return next(req.clone({ setHeaders: { Authorization: `Bearer ${newAccessToken}` } }));
                        }),
                        catchError(() => {
                            isRefreshing = false;
                            auth.logout();
                            return throwError(() => error);
                        })
                    );
                }

                return refreshSubject.pipe(
                    filter(token => token !== null),
                    take(1),
                    switchMap(token => next(req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }))
                    ));
            }
            return throwError(() => error);
        })
    );
};
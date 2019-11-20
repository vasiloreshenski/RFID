import { NavigationService } from './../service/navigation-service';
import { AuthService } from 'src/app/service/auth-service';
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class UnAuthorizedHttpInterceptor implements HttpInterceptor {
    constructor(private authService: AuthService, private navigationService: NavigationService) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(request).pipe(catchError(err => {
            if (err.status === 401) {
                this.authService.invalidateToken();
                this.navigationService.refresh();
            }
            // const error = err.statusText || err.error.message;
            return throwError(err);
        }));
    }
}

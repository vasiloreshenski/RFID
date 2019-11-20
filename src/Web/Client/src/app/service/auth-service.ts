import { Observable } from 'rxjs';
import { AuthTokenResponseModel, RfidHttpClient } from './rfid-http-client';
import { Injectable } from '@angular/core';

@Injectable()
export class AuthService {
    private static localStorageKey = 'authToken';

    constructor(private rfidHttp: RfidHttpClient) {
    }

    isAuthenticated(): boolean {
        if (localStorage.getItem(AuthService.localStorageKey)) {
            return true;
        }
        return false;
    }

    issueToken(email: string, password: string): Observable<AuthTokenResponseModel> {
        const authToken$ = this.rfidHttp.generateAuthToken(email, password);
        authToken$.subscribe(token => {
            localStorage.setItem(AuthService.localStorageKey, JSON.stringify(token));
        });

        return authToken$;
    }

    token(): string {
        if (this.isAuthenticated()) {
            const token = JSON.parse(localStorage.getItem(AuthService.localStorageKey)).token;

            return token;
        }

        return null;
    }

    invalidateToken() {
        if (this.isAuthenticated()) {
            localStorage.removeItem(AuthService.localStorageKey);
        }
    }
}

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
            const token = JSON.parse(localStorage.getItem(AuthService.localStorageKey));
            if (token.dateTimeExpirationInMs <= new Date().getUTCMilliseconds) {
                return false;
            }
            return true;
        }
        return false;
    }

    issueToken(email: string, password: string): Observable<AuthTokenResponseModel> {
        const obs$ = new Observable<AuthTokenResponseModel>((obs) => {
            const authToken$ = this.rfidHttp.generateAuthToken(email, password);
            authToken$.subscribe(token => {
                localStorage.setItem(AuthService.localStorageKey, JSON.stringify(token));
                obs.next(token);
            });
            authToken$.subscribe(_ => { });

            return { unsubscribe() { } };
        });

        return obs$;
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

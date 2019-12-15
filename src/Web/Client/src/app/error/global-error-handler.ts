import { RfidHttpClient } from './../service/rfid-http-client';
import { Injectable, ErrorHandler, Injector } from '@angular/core';
import { NotificationService } from '../service/notification-service';
import { PathLocationStrategy, LocationStrategy } from '@angular/common';
import { AppError } from './app-error';

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
    private notificationService: NotificationService;
    private httpClient: RfidHttpClient;

    constructor(private injector: Injector) {
        this.notificationService = injector.get(NotificationService);
        this.httpClient = injector.get(RfidHttpClient);
    }

    handleError(error: any): void {
        console.log(error);
        if (error instanceof AppError) {
            this.notificationService.error(error.message, 'Error');
        } else {
            this.notificationService.error('General error during user action. Please try again later.', 'Error');
        }
        const msg = error.message ? error.message : error.toString();
        const response$ = this.httpClient.logClientError(msg);
        // triger execution
        response$.subscribe(data => {});
        throw error;
    }
}

import { MatSnackBar } from '@angular/material/snack-bar';
import { Injectable } from '@angular/core';

@Injectable()
export class NotificationService {
    private accessPoint = 'Access point';
    constructor(private matSnackBar: MatSnackBar) { }

    public successLogin(username: string) {
        this.success(`Successfully logged in with ${username}`, '');
    }

    public successActivateAccessPoint() {
        this.success('Access point successfully activated', '');
    }

    public successDeleteAccessPoint() {
        this.success('Access point successfully deleted', '');
    }

    public successUpdateAccessPoint() {
        this.success('Access point successfully updated', '');
    }

    public successDeActivateAccessPoint() {
        this.success('Access point successfully de-activated', '');
    }

    public successRecoverDeletedAccessPoint() {
        this.success('Access point successfully recovered', '');
    }

    public successActivateTag() {
        this.success('Tag successfully activated', '');
    }

    public successDeActivateTag() {
        this.success('Tag successfully de-activate', '');
    }

    public successDeleteTag() {
        this.success('Tag successfully deleted', '');
    }

    public successRecoverTag() {
        this.success('Tag successfully recovered', '');
    }

    public successUpdateTag() {
        this.success('Tag successfully updated', '');
    }

    public notAuthorized() {
        this.error(`Not-authorized`, '');
    }

    public success(message: string, action: string) {
        this.matSnackBar.open(message, action, { panelClass: ['notification-success'] });
    }

    public error(message: string, action: string) {
        this.matSnackBar.open(message, action, { panelClass: ['notification-error'] });
    }
}

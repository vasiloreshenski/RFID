import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable()
export class NavigationService {
    static LoginRoute = '/admin/login';
    static StatRoute = '/admin/stat';
    static StatSummaryRoute = '/admin/stat/summary';
    static StatUserRoute = '/admin/stat/user';
    static AccessPointManagmentRoute = '/admin/accesspoint';
    static TagManagmentRoute = '/admin/tag';

    constructor(private router: Router) {
    }

    gotoLogin(): void {
        this.router.navigate([NavigationService.LoginRoute]);
    }

    gotoStat(): void {
        this.router.navigate([NavigationService.StatRoute]);
    }

    gotoAccessPoint(): void {
        this.router.navigate([NavigationService.AccessPointManagmentRoute]);
    }

    gotoTag(): void {
        this.router.navigate([NavigationService.TagManagmentRoute]);
    }

    gotoStatSummary(): void {
        this.router.navigate([NavigationService.StatSummaryRoute]);
    }

    gotoStatUser(): void {
        this.router.navigate([NavigationService.StatUserRoute]);
    }

    refresh(): void {
        location.reload();
    }
}

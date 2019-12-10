import { Injectable, EventEmitter } from '@angular/core';
import { Router, NavigationEnd, NavigationStart } from '@angular/router';
import { ProgressService } from './progress-service';

@Injectable()
export class NavigationService {
    static LoginRoute = '/admin/login';
    static StatRoute = '/admin/stat';
    static StatSummaryRoute = '/admin/stat/summary';
    static StatUserRoute = '/admin/stat/user';
    static AccessPointManagmentRoute = '/admin/accesspoint';
    static TagManagmentRoute = '/admin/tag';

    public NavigatedEvent: EventEmitter<String> = new EventEmitter<String>();

    constructor(private router: Router, private progressService: ProgressService) {
        router.events.subscribe(e => {
            if (e instanceof NavigationEnd) {
                this.NavigatedEvent.emit(this.currentPath());
            } else if (e instanceof NavigationStart) {
                this.progressService.startLoading();
            }
        });
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

    currentPath(): String {
        return this.router.url;
    }
}

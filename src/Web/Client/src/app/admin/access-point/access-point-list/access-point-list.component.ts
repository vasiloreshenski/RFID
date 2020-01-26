import { StateCount } from './../../../model/state-count';
import { Pagination } from './../../../model/pagination';
import { Observable } from 'rxjs';
import { UnknownAccessPoint } from '../../../model/unknown-access-point';
import { NavigationService } from '../../../service/navigation-service';
import { ModelFactory } from '../../../model/model-factory';
import { DirectionType } from '../../../model/direction-type';
import { AccessLevelType } from '../../../model/access-level-type';
import { RfidHttpClient } from '../../../service/rfid-http-client';
import { AccessPoint } from '../../../model/access-point';
import { Component, OnInit, Output, AfterViewInit, ViewChildren, QueryList, EventEmitter } from '@angular/core';
import { ProgressService } from 'src/app/service/progress-service';
import { PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'app-access-point-list',
  templateUrl: './access-point-list.component.html',
  styleUrls: ['./access-point-list.component.css']
})
export class AccessPointListComponent implements OnInit, AfterViewInit {
  public Title: string;
  public accessPointsPaginator: Pagination<AccessPoint> = Pagination.empty();
  public unknownAccessPointsPaginator: Pagination<UnknownAccessPoint> = Pagination.empty();
  public counts: StateCount = StateCount.empty();

  constructor(
    private rfidHttpClient: RfidHttpClient,
    private progressService: ProgressService) { }

  public reloadAccessPoints(ev: PageEvent): void {
    if (!ev) {
      ev = this.createPageEventFromPagination();
    }
    if (this.accessPointsPaginator.items.some(x => x.isDeleted)) {
      this.reloadDeletedAccessPoints(ev.pageIndex, ev.pageSize);
    } else if (this.accessPointsPaginator.items.some(x => x.isActive)) {
      this.reloadActiveAccessPoints(ev.pageIndex, ev.pageSize);
    } else if (this.accessPointsPaginator.items.length > 0) {
      this.reloadInActiveAccessPoints(ev.pageIndex, ev.pageSize);
    } else if (this.unknownAccessPointsPaginator.items.length > 0) {
      this.reloadUnKnownAccessPoints(ev.pageIndex, ev.pageSize);
    } else {
      this.reloadActiveAccessPoints(ev.pageIndex, ev.pageSize);
    }
    this.rfidHttpClient.getAccessPointCounts()
      .subscribe(data => {
        console.log(data);
        this.counts = data;
      });
  }

  public reloadActiveAccessPoints(page: number, pageSize: number): void {
    if (!page) {
      page = Pagination.defaultPage;
    }
    if (!pageSize) {
      pageSize = Pagination.defaultPageSize;
    }

    this.unknownAccessPointsPaginator = Pagination.empty();
    this.Title = 'Active';
    const obs$ = this.rfidHttpClient.getActiveAccessPoints(page, pageSize);
    this.progressService.executeWithProgress(obs$, data => {
      this.accessPointsPaginator = data;
    });
  }

  public reloadInActiveAccessPoints(page: number, pageSize: number): void {
    if (!page) {
      page = Pagination.defaultPage;
    }
    if (!pageSize) {
      pageSize = Pagination.defaultPageSize;
    }

    this.unknownAccessPointsPaginator = Pagination.empty();
    this.Title = 'In-Active';
    const obs$ = this.rfidHttpClient.getInActiveAccessPoints(page, pageSize);
    this.progressService.executeWithProgress(obs$, data => {
      this.accessPointsPaginator = data;
    });
  }

  public reloadDeletedAccessPoints(page: number, pageSize: number): void {
    if (!page) {
      page = Pagination.defaultPage;
    }
    if (!pageSize) {
      pageSize = Pagination.defaultPageSize;
    }

    this.unknownAccessPointsPaginator = Pagination.empty();
    this.Title = 'Deleted';
    const obs$ = this.rfidHttpClient.getDeletedAccessPoints(page, pageSize);
    this.progressService.executeWithProgress(obs$, data => {
      this.accessPointsPaginator = data;
    });
  }

  public reloadUnKnownAccessPoints(page: number, pageSize: number): void {
    if (!page) {
      page = Pagination.defaultPage;
    }
    if (!pageSize) {
      pageSize = Pagination.defaultPageSize;
    }

    this.Title = 'Unknown';
    this.accessPointsPaginator = Pagination.empty();
    const obs$ = this.rfidHttpClient.getUnknownAccessPoints(page, pageSize);
    this.progressService.executeWithProgress(obs$, data => {
      this.unknownAccessPointsPaginator = data;
    });
  }

  private createPageEventFromPagination(): PageEvent {
    if (this.unknownAccessPointsPaginator.items.length > 0) {
      return this.unknownAccessPointsPaginator.createPageEvent();
    } else {
      return this.accessPointsPaginator.createPageEvent();
    }
  }

  ngOnInit() {
    this.reloadAccessPoints(null);
  }

  ngAfterViewInit(): void {
  }
}

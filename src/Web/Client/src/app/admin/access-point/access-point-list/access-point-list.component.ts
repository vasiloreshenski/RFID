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

@Component({
  selector: 'app-access-point-list',
  templateUrl: './access-point-list.component.html',
  styleUrls: ['./access-point-list.component.css']
})
export class AccessPointListComponent implements OnInit, AfterViewInit {
  public Title: string;
  public accessPoints: AccessPoint[] = [];
  public unknownAccessPoints: UnknownAccessPoint[] = [];

  constructor(
    private rfidHttpClient: RfidHttpClient,
    private navigationService: NavigationService,
    private progressService: ProgressService) { }

  public reloadAccessPoints(): void {
    if (this.accessPoints.some(x => x.isDeleted)) {
      this.reloadDeletedAccessPoints();
    } else if (this.accessPoints.some(x => x.isActive)) {
      this.reloadActiveAccessPoints();
    } else if (this.accessPoints.length > 0) {
      this.reloadInActiveAccessPoints();
    } else if (this.unknownAccessPoints.length > 0) {
      this.reloadUnKnownAccessPoints();
    } else {
      this.reloadActiveAccessPoints();
    }
  }

  public reloadActiveAccessPoints(): void {
    this.unknownAccessPoints = [];
    this.Title = 'Active';
    const obs$ = this.rfidHttpClient.getActiveAccessPoints();
    this.progressService.executeWithProgress(obs$, data => {
      this.accessPoints = [];
      this.accessPoints.push(...data);
    });
  }

  public reloadInActiveAccessPoints(): void {
    this.unknownAccessPoints = [];
    this.Title = 'In-Active';
    const obs$ = this.rfidHttpClient.getInActiveAccessPoints();
    this.progressService.executeWithProgress(obs$, data => {
      this.accessPoints = [];
      this.accessPoints.push(...data);
    });
  }

  public reloadDeletedAccessPoints(): void {
    this.unknownAccessPoints = [];
    this.Title = 'Deleted';
    const obs$ = this.rfidHttpClient.getInDeletedAccessPoints();
    this.progressService.executeWithProgress(obs$, data => {
      this.accessPoints = [];
      this.accessPoints.push(...data);
    });
  }

  public reloadUnKnownAccessPoints(): void {
    this.Title = 'Unknown';
    this.accessPoints = [];
    const obs$ = this.rfidHttpClient.getUnknownAccessPoints();
    this.progressService.executeWithProgress(obs$, data => {
      this.unknownAccessPoints = [];
      this.unknownAccessPoints.push(...data);
    });
  }

  ngOnInit() {
    this.reloadAccessPoints();
  }

  ngAfterViewInit(): void {
  }
}

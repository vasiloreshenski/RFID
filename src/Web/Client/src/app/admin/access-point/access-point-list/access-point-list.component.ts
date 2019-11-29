import { UnknownAccessPoint } from '../../../model/unknown-access-point';
import { NavigationService } from '../../../service/navigation-service';
import { ModelFactory } from '../../../model/model-factory';
import { DirectionType } from '../../../model/direction-type';
import { AccessLevelType } from '../../../model/access-level-type';
import { RfidHttpClient } from '../../../service/rfid-http-client';
import { AccessPoint } from '../../../model/access-point';
import { Component, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-access-point-list',
  templateUrl: './access-point-list.component.html',
  styleUrls: ['./access-point-list.component.css']
})
export class AccessPointListComponent implements OnInit {

  public Title: string;
  public accessPoints: AccessPoint[] = [];
  public unknownAccessPoints: UnknownAccessPoint[] = [];

  constructor(private rfidHttpClient: RfidHttpClient, private navigationService: NavigationService) { }

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
    this.rfidHttpClient.getActiveAccessPoints().subscribe(
      data => {
        this.accessPoints = [];
        this.accessPoints.push(...data);
      },
      error => console.log(error)
    );
  }

  public reloadInActiveAccessPoints(): void {
    this.unknownAccessPoints = [];
    this.Title = 'In-Active';
    this.rfidHttpClient.getInActiveAccessPoints().subscribe(
      data => {
        this.accessPoints = [];
        this.accessPoints.push(...data);
      },
      error => console.log(error)
    );
  }

  public reloadDeletedAccessPoints(): void {
    this.unknownAccessPoints = [];
    this.Title = 'Deleted';
    this.rfidHttpClient.getInDeletedAccessPoints().subscribe(
      data => {
        this.accessPoints = [];
        this.accessPoints.push(...data);
      },
      error => console.log(error)
    );
  }

  public reloadUnKnownAccessPoints(): void {
    this.Title = 'Unknown';
    this.accessPoints = [];
    this.rfidHttpClient.getUnknownAccessPoints().subscribe(
      data => {
        this.unknownAccessPoints = [];
        this.unknownAccessPoints.push(...data);
      },
      error => console.log(error)
    );
  }

  ngOnInit() {
    this.reloadAccessPoints();
  }
}

import { UnknownAccessPoint } from '../../../model/unknown-access-point';
import { NavigationService } from '../../../service/navigation-service';
import { ModelFactory } from '../../../model/model-factory';
import { DirectionType } from '../../../model/direction-type';
import { AccessLevelType } from '../../../model/access-level-type';
import { RfidHttpClient } from '../../../service/rfid-http-client';
import { AccessPoint } from '../../../model/access-point';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-access-point-list',
  templateUrl: './access-point-list.component.html',
  styleUrls: ['./access-point-list.component.css']
})
export class AccessPointListComponent implements OnInit {
  public activeAccessPoints: AccessPoint[] = [];
  public inActiveAccessPoints: AccessPoint[] = [];
  public unknownAccessPoints: UnknownAccessPoint[] = [];

  constructor(private rfidHttpClient: RfidHttpClient, private navigationService: NavigationService) { }

  public reloadAccessPoints(): void {
    this.reloadActiveAccessPoints();
    this.reloadInActiveAccessPoints();
    this.reloadUnKnownAccessPoints();
  }

  public reloadActiveAccessPoints(): void {
    this.rfidHttpClient.getActiveAccessPoints().subscribe(
      data => {
        this.activeAccessPoints = [];
        this.activeAccessPoints.push(...data);
      },
      error => console.log(error)
    );
  }

  public reloadInActiveAccessPoints(): void {
    this.rfidHttpClient.getInActiveAccessPoints().subscribe(
      data => {
        this.inActiveAccessPoints = [];
        this.inActiveAccessPoints.push(...data);
      },
      error => console.log(error)
    );
  }

  public reloadUnKnownAccessPoints(): void {
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

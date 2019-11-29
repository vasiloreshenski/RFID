import { HtmlService } from './../../../service/html-service';
import { AccessPoint } from './../../../model/access-point';
import { RfidHttpClient } from '../../../service/rfid-http-client';
import { Component, OnInit, Input, EventEmitter, Output, ChangeDetectorRef } from '@angular/core';
import { AccessLevelType } from 'src/app/model/access-level-type';
import { DirectionType } from 'src/app/model/direction-type';
import { ModelFactory } from 'src/app/model/model-factory';
import { subscribeOn } from 'rxjs/operators';
import { Validators, FormControl } from '@angular/forms';

@Component({
  selector: 'app-access-point-item',
  templateUrl: './access-point-item.component.html',
  styleUrls: ['./access-point-item.component.css']
})
export class AccessPointItemComponent implements OnInit {
  public accessLevels: string[] = [
    AccessLevelType[AccessLevelType.Low], AccessLevelType[AccessLevelType.Mid], AccessLevelType[AccessLevelType.High]
  ];
  public directions: string[] = [
    DirectionType[DirectionType.Entrance], DirectionType[DirectionType.Exit]
  ];

  private beforeEditAccessPoint: AccessPoint;

  @Input()
  public accessPoint: AccessPoint;

  @Output()
  public accessPointUpdate = new EventEmitter();

  public descriptionControl = new FormControl('', [Validators.required, Validators.minLength(1)]);
  public accessLevelControl = new FormControl('');
  public directionControl = new FormControl('');

  constructor(private rfidHttpClient: RfidHttpClient) { }

  ngOnInit() {
    this.enableDisableControls();
    this.initControls();
  }

  public registerOrUpdate() {
    if (this.accessPoint.isRegistered()) {
      const requestModel = ModelFactory.updateAccessPointRequestModel(
        this.accessPoint.id,
        this.descriptionControl.value,
        this.accessLevelControl.value,
        this.directionControl.value,
        this.accessPoint.isActive
      );
      this.rfidHttpClient.updateAccessPoint(requestModel).subscribe(
        data => this.accessPointUpdate.emit(),
        error => console.log(error)
      );
    } else {
      const requestModel = ModelFactory.registerAccessPointRequestModel(
        this.accessPoint.serialNumber,
        this.descriptionControl.value,
        this.accessLevelControl.value,
        this.directionControl.value,
        this.accessPoint.isActive
      );
      this.rfidHttpClient.registerAccessPoint(requestModel).subscribe(
        data => this.accessPointUpdate.emit(),
        error => console.log(error)
      );
    }
    return false;
  }

  public updateActivateStatus() {
    if (this.accessPoint.isActive) {
      this.rfidHttpClient.deActivateAccessPoint(this.accessPoint.id).subscribe(
        data => this.accessPointUpdate.emit(),
        error => console.log(error)
      );
    } else {
      this.rfidHttpClient.activateAccessPoint(this.accessPoint.id).subscribe(
        data => this.accessPointUpdate.emit(),
        error => console.log(error)
      );
    }
    return false;
  }

  public delete() {
    if (this.accessPoint.isRegistered()) {
      this.rfidHttpClient.deleteAccessPoint(this.accessPoint.id).subscribe(
        data => this.accessPointUpdate.emit(),
        error => console.log(error)
      );
    }
    return false;
  }

  public unDelete() {
    if (this.accessPoint.isDeleted) {
      this.rfidHttpClient.unDeleteAccessPoint(this.accessPoint.id).subscribe(
        data => this.accessPointUpdate.emit(),
        error => console.log(error)
      );
    }
    return false;
  }

  public enableEdit() {
    this.accessPoint.canEdit = true;
    this.enableDisableControls();
  }

  public cancelEdit() {
    this.accessPoint.canEdit = false;
    this.enableDisableControls();
    this.initControls();
  }

  private enableDisableControls() {
    if (this.accessPoint.canEdit) {
      this.descriptionControl.enable();
      this.accessLevelControl.enable();
      this.directionControl.enable();
    } else {
      this.descriptionControl.disable();
      this.accessLevelControl.disable();
      this.directionControl.disable();
    }
  }

  private initControls() {
    this.descriptionControl.setValue(this.accessPoint.description);
    this.accessLevelControl.setValue(this.accessPoint.getAccessLevelText());
    this.directionControl.setValue(this.accessPoint.getDirectionText());
  }
}

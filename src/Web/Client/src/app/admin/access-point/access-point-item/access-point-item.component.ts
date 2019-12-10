import { ProgressService } from 'src/app/service/progress-service';
import { HtmlService } from './../../../service/html-service';
import { AccessPoint } from './../../../model/access-point';
import { RfidHttpClient } from '../../../service/rfid-http-client';
import { Component, OnInit, Input, EventEmitter, Output, ChangeDetectorRef } from '@angular/core';
import { AccessLevelType } from 'src/app/model/access-level-type';
import { DirectionType } from 'src/app/model/direction-type';
import { ModelFactory } from 'src/app/model/model-factory';
import { Validators, FormControl } from '@angular/forms';
import { NotificationService } from 'src/app/service/notification-service';

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

  constructor(
    private rfidHttpClient: RfidHttpClient,
    private notificationService: NotificationService,
    private progressService: ProgressService) { }

  ngOnInit() {
    this.enableDisableControls();
    this.initControls();
  }

  public registerOrUpdate() {
    if (this.areControlsValid() === false) {
      return false;
    }

    if (this.accessPoint.isRegistered()) {
      const requestModel = ModelFactory.updateAccessPointRequestModel(
        this.accessPoint.id,
        this.descriptionControl.value,
        this.accessLevelControl.value,
        this.directionControl.value,
        this.accessPoint.isActive
      );
      this.progressService.executeWithProgress(this.rfidHttpClient.updateAccessPoint(requestModel), data => {
        this.accessPointUpdate.emit();
        this.notificationService.successUpdateAccessPoint();
      });
    } else {
      const requestModel = ModelFactory.registerAccessPointRequestModel(
        this.accessPoint.serialNumber,
        this.descriptionControl.value,
        this.accessLevelControl.value,
        this.directionControl.value,
        this.accessPoint.isActive
      );
      this.progressService.executeWithProgress(this.rfidHttpClient.registerAccessPoint(requestModel), data => {
        this.accessPointUpdate.emit();
        this.notificationService.successUpdateAccessPoint();
      });
    }
    return false;
  }

  public updateActivateStatus() {
    if (this.accessPoint.isActive) {
      this.progressService.executeWithProgress(this.rfidHttpClient.deActivateAccessPoint(this.accessPoint.id), data => {
        this.accessPointUpdate.emit();
        this.notificationService.successDeActivateAccessPoint();
      });
    } else {
      this.progressService.executeWithProgress(this.rfidHttpClient.activateAccessPoint(this.accessPoint.id), data => {
        this.accessPointUpdate.emit();
        this.notificationService.successActivateAccessPoint();
      });
    }
    return false;
  }

  public delete() {
    if (this.accessPoint.isRegistered()) {
      this.progressService.executeWithProgress(this.rfidHttpClient.deleteAccessPoint(this.accessPoint.id), data => {
        this.accessPointUpdate.emit();
        this.notificationService.successDeleteAccessPoint();
      });
    }
    return false;
  }

  public unDelete() {
    if (this.accessPoint.isDeleted) {
      this.progressService.executeWithProgress(this.rfidHttpClient.unDeleteAccessPoint(this.accessPoint.id), data => {
        this.accessPointUpdate.emit();
        this.notificationService.successRecoverDeletedAccessPoint();
      });
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

  public areControlsValid(): boolean {
    return this.descriptionControl.valid && this.accessLevelControl.valid && this.directionControl.valid;
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

    // to simualte human interaction which will trigger form validation
    this.descriptionControl.markAsTouched();
  }
}

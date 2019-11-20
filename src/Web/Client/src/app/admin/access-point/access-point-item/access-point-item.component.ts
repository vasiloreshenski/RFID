import { HtmlService } from './../../../service/html-service';
import { AccessPoint } from './../../../model/access-point';
import { RfidHttpClient } from '../../../service/rfid-http-client';
import { Component, OnInit, Input, EventEmitter, Output, ChangeDetectorRef } from '@angular/core';
import { AccessLevelType } from 'src/app/model/access-level-type';
import { DirectionType } from 'src/app/model/direction-type';
import { ModelFactory } from 'src/app/model/model-factory';
import { subscribeOn } from 'rxjs/operators';

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

  constructor(private rfidHttpClient: RfidHttpClient) { }

  ngOnInit() {
  }

  public registerOrUpdate(description: string, accessLevelText: string, directionText: string) {
    if (this.accessPoint.isRegistered()) {
      const requestModel = ModelFactory.updateAccessPointRequestModel(
        this.accessPoint.id, description, accessLevelText, directionText, this.accessPoint.isActive
      );
      this.rfidHttpClient.updateAccessPoint(requestModel).subscribe(
        data => this.accessPointUpdate.emit(),
        error => console.log(error)
      );
    } else {
      const requestModel = ModelFactory.registerAccessPointRequestModel(
        this.accessPoint.serialNumber, description, accessLevelText, directionText, this.accessPoint.isActive
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

  public enableEdit() {
    this.accessPoint.canEdit = true;
  }

  public cancelEdit(accessLevelSelect: HTMLSelectElement, directionSelect: HTMLSelectElement, descriptionTextArea: HTMLTextAreaElement) {
    this.accessPoint.canEdit = false;
    // refresh state
    HtmlService.selectOption(accessLevelSelect, this.accessPoint.getAccessLevelText());
    HtmlService.selectOption(directionSelect, this.accessPoint.getDirectionText());
    descriptionTextArea.value = this.accessPoint.description;
  }
}

import { FormControl, Validators } from '@angular/forms';
import { HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HtmlService } from './../../../service/html-service';
import { RfidHttpClient } from './../../../service/rfid-http-client';
import { ModelFactory } from 'src/app/model/model-factory';
import { TagUser } from 'src/app/model/tag-user';
import { AccessLevelType } from './../../../model/access-level-type';
import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { Tag } from 'src/app/model/tag';
import { startWith, map } from 'rxjs/operators';
import { NotificationService } from 'src/app/service/notification-service';
import { ProgressService } from 'src/app/service/progress-service';

@Component({
  selector: 'app-tag-item',
  templateUrl: './tag-item.component.html',
  styleUrls: ['./tag-item.component.css']
})
export class TagItemComponent implements OnInit {
  @Output()
  public tagUpdateEvent = new EventEmitter();

  @Input()
  public tag: Tag;

  @Input()
  public users: TagUser[];

  public accessLevels: String[] = [
    AccessLevelType[AccessLevelType.Low], AccessLevelType[AccessLevelType.Mid], AccessLevelType[AccessLevelType.High]
  ];

  public filteredUsers: Observable<TagUser[]>;

  public userControl = new FormControl('', [Validators.required]);
  public accessLevelControl = new FormControl('');

  constructor(
    private rfidHttp: RfidHttpClient,
    private notificationService: NotificationService,
    private progressService: ProgressService) { }

  public edit() {
    this.tag.canEdit = true;
    this.enableDisableControls();
    return false;
  }

  public cancel() {
    this.tag.canEdit = false;
    this.enableDisableControls();
    this.initControls();
    return false;
  }

  public save() {
    const userName = this.userControl.value;
    const accessLevel = this.accessLevelControl.value;
    let httpResponse$: Observable<HttpResponse<Object>>;
    if (this.tag.isRegistered()) {
      const requestModel = ModelFactory.updateTagRequestModel(this.tag.id, userName, accessLevel);
      httpResponse$ = this.rfidHttp.updateTag(requestModel);
    } else {
      const requestModel = ModelFactory.registerTagRequestModel(this.tag.number, userName, accessLevel);
      httpResponse$ = this.rfidHttp.registerTag(requestModel);
    }

    this.progressService.executeWithProgress(httpResponse$, data => {
      this.tagUpdateEvent.emit();
      this.notificationService.successUpdateTag();
    });
    return false;
  }

  public activateOrDeActivate() {
    let httpResponse$: Observable<HttpResponse<Object>>;
    if (this.tag.isActive) {
      httpResponse$ = this.rfidHttp.deActivateTag(this.tag.id);
    } else {
      httpResponse$ = this.rfidHttp.activateTag(this.tag.id);
    }
    this.progressService.executeWithProgress(httpResponse$, data => {
      this.tagUpdateEvent.emit();
      if (this.tag.isActive) {
        this.notificationService.successDeActivateTag();
      } else {
        this.notificationService.successActivateTag();
      }
    });

    return false;
  }

  public delete() {
    this.progressService.executeWithProgress(this.rfidHttp.deleteTag(this.tag.id), data => {
      this.tagUpdateEvent.emit();
      this.notificationService.successDeleteTag();
    });
    return false;
  }

  public unDelete() {
    this.progressService.executeWithProgress(this.rfidHttp.unDeleteTag(this.tag.id), data => {
      this.tagUpdateEvent.emit();
      this.notificationService.successRecoverTag();
    });
    return false;
  }

  public areControlsValid(): boolean {
    return this.userControl.valid && this.accessLevelControl.valid;
  }

  ngOnInit() {
    this.initControls();
    this.enableDisableControls();

    this.filteredUsers = this.userControl.valueChanges.pipe(
      startWith(''),
      map(value => this.users.filter(u => u.userName.toLowerCase().includes(value)))
    );
  }

  private initControls(): void {
    this.userControl.setValue(this.tag.userName);
    this.accessLevelControl.setValue(this.tag.getAccessLevelDisplayText());
    this.userControl.markAsTouched();
  }

  private enableDisableControls(): void {
    if (this.tag.canEdit) {
      this.userControl.enable();
      this.accessLevelControl.enable();
    } else {
      this.userControl.disable();
      this.accessLevelControl.disable();
    }
  }
}

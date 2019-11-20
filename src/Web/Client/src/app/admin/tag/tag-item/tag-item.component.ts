import { HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HtmlService } from './../../../service/html-service';
import { RfidHttpClient } from './../../../service/rfid-http-client';
import { ModelFactory } from 'src/app/model/model-factory';
import { TagUser } from 'src/app/model/tag-user';
import { AccessLevelType } from './../../../model/access-level-type';
import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { Tag } from 'src/app/model/tag';

@Component({
  selector: 'app-tag-item',
  templateUrl: './tag-item.component.html',
  styleUrls: ['./tag-item.component.css']
})
export class TagItemComponent implements OnInit {
  public accessLevels: String[] = [
    AccessLevelType[AccessLevelType.Low], AccessLevelType[AccessLevelType.Mid], AccessLevelType[AccessLevelType.High]
  ];

  @Output()
  public tagUpdateEvent = new EventEmitter();

  @Input()
  public tag: Tag;

  @Input()
  public users: TagUser[];

  constructor(private rfidHttp: RfidHttpClient) { }

  public edit() {
    this.tag.editMode = true;
    return false;
  }

  public cancel(userNameInput: HTMLInputElement, accessLevelSelect: HTMLSelectElement) {
    this.tag.editMode = false;
    userNameInput.value = this.tag.userName;
    HtmlService.selectOption(accessLevelSelect, this.tag.getAccessLevelDisplayText());
    return false;
  }

  public save(userName: string, accessLevel: string) {
    let httpResponse$: Observable<HttpResponse<Object>>;
    if (this.tag.isRegistered()) {
      const requestModel = ModelFactory.updateTagRequestModel(this.tag.id, userName, accessLevel);
      httpResponse$ = this.rfidHttp.updateTag(requestModel);
    } else {
      const requestModel = ModelFactory.registerTagRequestModel(this.tag.number, userName, accessLevel);
      httpResponse$ = this.rfidHttp.registerTag(requestModel);
    }

    httpResponse$.subscribe(
      data => this.tagUpdateEvent.emit(),
      error => console.log(error)
    );

    return false;
  }

  public activateOrDeActivate() {
    let httpResponse$: Observable<HttpResponse<Object>>;
    if (this.tag.isActive) {
      httpResponse$ = this.rfidHttp.deActivateTag(this.tag.id);
    } else {
      httpResponse$ = this.rfidHttp.activateTag(this.tag.id);
    }
    httpResponse$.subscribe(
      data => this.tagUpdateEvent.emit(),
      error => console.log(error)
    );
    return false;
  }

  public delete() {
    this.rfidHttp.deleteTag(this.tag.id).subscribe(
      data => this.tagUpdateEvent.emit(),
      error => console.log(error)
    );
    return false;
  }

  ngOnInit() {
  }
}

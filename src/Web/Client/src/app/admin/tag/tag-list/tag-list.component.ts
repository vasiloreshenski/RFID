import { UnknownTag } from './../../../model/unknown-tag';
import { RfidHttpClient } from './../../../service/rfid-http-client';
import { Component, OnInit } from '@angular/core';
import { Tag } from 'src/app/model/tag';
import { TagUser } from 'src/app/model/tag-user';

@Component({
  selector: 'app-tag-list',
  templateUrl: './tag-list.component.html',
  styleUrls: ['./tag-list.component.css']
})
export class TagListComponent implements OnInit {
  public active: Tag[] = [];
  public inActive: Tag[] = [];
  public unknown: UnknownTag[] = [];
  public users: TagUser[] = [];

  constructor(private rfidHttpClient: RfidHttpClient) {

  }

  public reload(): void {
    this.reloadActiveTags();
    this.reloadUsers();
    this.reloadInActiveTags();
    this.reloadUnknownTags();
  }

  private reloadActiveTags(): void {
    this.rfidHttpClient.getActiveTags().subscribe(
      data => {
        this.active = [];
        this.active.push(...data);
      },
      error => console.log(error)
    );
  }

  private reloadInActiveTags(): void {
    this.rfidHttpClient.getInActiveTags().subscribe(
      data => {
        this.inActive = [];
        this.inActive.push(...data);
      },
      error => console.log(error)
    );
  }

  private reloadUnknownTags(): void {
    this.rfidHttpClient.getUnknownTags().subscribe(
      data => {
        this.unknown = [];
        this.unknown.push(...data);
      },
      error => console.log(error)
    );
  }

  private reloadUsers(): void {
    this.rfidHttpClient.getTagsUsers().subscribe(
      data => {
        this.users = [];
        this.users.push(...data);
      },
      error => console.log(error)
    );
  }

  ngOnInit() {
    this.reload();
  }
}

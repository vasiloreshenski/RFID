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
  public tags: Tag[] = [];
  // public inActive: Tag[] = [];
  public unknown: UnknownTag[] = [];
  public users: TagUser[] = [];
  public Title: String;

  constructor(private rfidHttpClient: RfidHttpClient) {

  }

  public reload(): void {
    if (this.tags.some(t => t.isDeleted)) {
      this.reloadDeletedTags();
    } else if (this.tags.some(t => t.isActive === false)) {
      this.reloadInActiveTags();
    } else if (this.unknown.length > 0) {
      this.reloadUnknownTags();
    } else {
      this.reloadActiveTags();
    }
    this.reloadUsers();
  }

  public reloadActiveTags(): void {
    this.Title = 'Active';
    this.unknown = [];
    this.rfidHttpClient.getActiveTags().subscribe(
      data => {
        this.tags = [];
        this.tags.push(...data);
      },
      error => console.log(error)
    );
  }

  public reloadInActiveTags(): void {
    this.Title = 'In-Active';
    this.unknown = [];
    this.rfidHttpClient.getInActiveTags().subscribe(
      data => {
        this.tags = [];
        this.tags.push(...data);
      },
      error => console.log(error)
    );
  }

  public reloadUnknownTags(): void {
    this.Title = 'Unknown';
    this.tags = [];
    this.rfidHttpClient.getUnknownTags().subscribe(
      data => {
        this.unknown = [];
        this.unknown.push(...data);
      },
      error => console.log(error)
    );
  }

  public reloadDeletedTags(): void {
    this.Title = 'Deleted';
    this.unknown = [];
    this.rfidHttpClient.getDeletedTags().subscribe(
      data => {
        this.tags = [];
        this.tags.push(...data);
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

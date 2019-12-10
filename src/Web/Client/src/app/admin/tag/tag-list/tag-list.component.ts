import { ProgressService } from 'src/app/service/progress-service';
import { startWith } from 'rxjs/operators';
import { Observable } from 'rxjs';
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

  constructor(
    private rfidHttpClient: RfidHttpClient,
    private progressService: ProgressService) {

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
    const obs$ = this.rfidHttpClient.getActiveTags();
    this.progressService.executeWithProgress(obs$, data => {
      this.tags = [];
      this.tags.push(...data);
    });
  }

  public reloadInActiveTags(): void {
    this.Title = 'In-Active';
    this.unknown = [];
    const obs$ = this.rfidHttpClient.getInActiveTags();
    this.progressService.executeWithProgress(obs$, data => {
      this.tags = [];
      this.tags.push(...data);
    });
  }

  public reloadUnknownTags(): void {
    this.Title = 'Unknown';
    this.tags = [];
    const obs$ = this.rfidHttpClient.getUnknownTags();
    this.progressService.executeWithProgress(obs$, data => {
      this.unknown = [];
      this.unknown.push(...data);
    });
  }

  public reloadDeletedTags(): void {
    this.Title = 'Deleted';
    this.unknown = [];
    const obs$ = this.rfidHttpClient.getDeletedTags();
    this.progressService.executeWithProgress(obs$, data => {
      this.tags = [];
      this.tags.push(...data);
    });
  }

  private reloadUsers(): void {
    this.rfidHttpClient.getTagsUsers().subscribe(
      data => {
        this.users = [];
        this.users.push(...data);
      });
  }

  ngOnInit() {
    this.reload();
  }
}

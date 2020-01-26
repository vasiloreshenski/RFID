import { StateCount } from './../../../model/state-count';
import { PageEvent } from '@angular/material/paginator';
import { Pagination } from './../../../model/pagination';
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
  public tagPaginatior: Pagination<Tag> = Pagination.empty();
  public unknownPaginator: Pagination<UnknownTag> = Pagination.empty();
  public users: TagUser[] = [];
  public Title: String;
  public counts: StateCount = StateCount.empty();

  constructor(
    private rfidHttpClient: RfidHttpClient,
    private progressService: ProgressService) {
  }

  public reload(ev: PageEvent): void {
    if (!ev) {
      ev = this.createPageEventFromPagination();
    }
    if (this.tagPaginatior.items.some(t => t.isDeleted)) {
      this.reloadDeletedTags(ev.pageIndex, ev.pageSize);
    } else if (this.tagPaginatior.items.some(t => t.isActive === false)) {
      this.reloadInActiveTags(ev.pageIndex, ev.pageSize);
    } else if (this.unknownPaginator.items.length > 0) {
      this.reloadUnknownTags(ev.pageIndex, ev.pageSize);
    } else {
      this.reloadActiveTags(ev.pageIndex, ev.pageSize);
    }
    this.reloadUsers();
    this.reloadCounts();
  }

  public reloadActiveTags(page: number, pageSize: number): void {
    if (!page) {
      page = Pagination.defaultPage;
    }

    if (!pageSize) {
      pageSize = Pagination.defaultPageSize;
    }

    this.Title = 'Active';
    this.unknownPaginator = Pagination.empty();
    const obs$ = this.rfidHttpClient.getActiveTags(page, pageSize);
    this.progressService.executeWithProgress(obs$, data => {
      this.tagPaginatior = data;
    });
  }

  public reloadInActiveTags(page: number, pageSize: number): void {
    if (!page) {
      page = Pagination.defaultPage;
    }

    if (!pageSize) {
      pageSize = Pagination.defaultPageSize;
    }

    this.Title = 'In-Active';
    this.unknownPaginator = Pagination.empty();
    const obs$ = this.rfidHttpClient.getInActiveTags(page, pageSize);
    this.progressService.executeWithProgress(obs$, data => {
      this.tagPaginatior = data;
    });
  }

  public reloadUnknownTags(page: number, pageSize: number): void {
    if (!page) {
      page = Pagination.defaultPage;
    }

    if (!pageSize) {
      pageSize = Pagination.defaultPageSize;
    }

    this.Title = 'Unknown';
    this.tagPaginatior = Pagination.empty();
    const obs$ = this.rfidHttpClient.getUnknownTags(page, pageSize);
    this.progressService.executeWithProgress(obs$, data => {
      this.unknownPaginator = data;
    });
  }

  public reloadDeletedTags(page: number, pageSize: number): void {
    if (!page) {
      page = Pagination.defaultPage;
    }

    if (!pageSize) {
      pageSize = Pagination.defaultPageSize;
    }

    this.Title = 'Deleted';
    this.unknownPaginator = Pagination.empty();
    const obs$ = this.rfidHttpClient.getDeletedTags(page, pageSize);
    this.progressService.executeWithProgress(obs$, data => {
      this.tagPaginatior = data;
    });
  }

  private reloadUsers(): void {
    this.rfidHttpClient.getTagsUsers().subscribe(
      data => {
        this.users = [];
        this.users.push(...data);
      });
  }

  private reloadCounts(): void {
    this.rfidHttpClient.getTagsCounts().subscribe(data => this.counts = data);
  }

  private createPageEventFromPagination(): PageEvent {
    if (this.unknownPaginator.items.length > 0) {
      return this.unknownPaginator.createPageEvent();
    } else {
      return this.tagPaginatior.createPageEvent();
    }
  }

  ngOnInit() {
    this.reload(null);
  }
}

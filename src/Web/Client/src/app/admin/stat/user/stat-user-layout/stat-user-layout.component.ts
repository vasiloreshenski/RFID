import { Component, OnInit } from '@angular/core';
import { StatUser } from 'src/app/model/stat-user';

@Component({
  selector: 'app-stat-user-layout',
  templateUrl: './stat-user-layout.component.html',
  styleUrls: ['./stat-user-layout.component.css']
})
export class StatUserLayoutComponent implements OnInit {

  public selectedUser: StatUser;
  public isOverviewLoading = true;
  public isUserListLoading = true;

  constructor() { }

  public setSelectedUser(user: StatUser) {
    this.selectedUser = user;
  }

  public setIsOverviewLoading(value: boolean) {
    console.log('overview loading: ' + value);
    this.isOverviewLoading = value;
  }

  public setIsUserListLoading(value: boolean) {
    console.log('user list loading: ' + value);
    this.isUserListLoading = value;
  }

  ngOnInit() {
  }
}

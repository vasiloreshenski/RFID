import { StatUser } from './../../../../model/stat-user';
import { RfidHttpClient } from './../../../../service/rfid-http-client';
import { Component, OnInit, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-stat-user-list',
  templateUrl: './stat-user-list.component.html',
  styleUrls: ['./stat-user-list.component.css']
})
export class StatUserListComponent implements OnInit {

  @Output()
  public selectedUserEvent: EventEmitter<StatUser> = new EventEmitter<StatUser>();
  public selectedUser: StatUser;
  public users: StatUser[] = [];

  constructor(private rfidHttpClient: RfidHttpClient) { }

  public setSelectedUser(user: StatUser) {
    if (this.selectedUser) {
      this.selectedUser.isSelected = false;
    }

    this.selectedUser = user;
    this.selectedUser.isSelected = true;
    this.selectedUserEvent.emit(this.selectedUser);
  }

  ngOnInit() {
    this.rfidHttpClient.getStatUsers().subscribe(
      data => {
        this.users = [];
        this.users.push(...data);
      },
      error => console.log(error)
    );
  }
}

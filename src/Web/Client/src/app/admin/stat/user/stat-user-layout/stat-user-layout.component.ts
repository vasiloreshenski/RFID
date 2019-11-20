import { Component, OnInit } from '@angular/core';
import { StatUser } from 'src/app/model/stat-user';

@Component({
  selector: 'app-stat-user-layout',
  templateUrl: './stat-user-layout.component.html',
  styleUrls: ['./stat-user-layout.component.css']
})
export class StatUserLayoutComponent implements OnInit {

  public selectedUser: StatUser;

  constructor() { }

  public setSelectedUser(user: StatUser) {
    this.selectedUser = user;
  }

  ngOnInit() {
  }
}

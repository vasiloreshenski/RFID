import { StatUserOverview } from './../../../../model/stat-user-overview';
import { RfidHttpClient } from './../../../../service/rfid-http-client';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-stat-user-overview',
  templateUrl: './stat-user-overview.component.html',
  styleUrls: ['./stat-user-overview.component.css']
})
export class StatUserOverviewComponent implements OnInit {

  public overview: StatUserOverview;

  constructor(private rfidHttpClient: RfidHttpClient) { }

  ngOnInit() {
    this.overview = new StatUserOverview();
    this.rfidHttpClient.getStatUserOverview().subscribe(
      data => this.overview = data,
      error => console.log(error)
    );
  }
}

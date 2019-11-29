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
  public data: any[] = [];

  constructor(private rfidHttpClient: RfidHttpClient) { }

  ngOnInit() {
    this.overview = new StatUserOverview();
    this.rfidHttpClient.getStatUserOverview().subscribe(
      response => {
        this.overview = response;
        this.data = [];
        this.data.push(
          { 'name': 'Avg. entrance time', 'value': response.avgEntranceTime.time },
          { 'name': 'Avg. exit time', 'value': response.avgExitTime.time },
          { 'name': 'Avg. work hour norm', 'value': response.avgWorkHourNorm.time }
        );
      },
      error => console.log(error)
    );
  }
}

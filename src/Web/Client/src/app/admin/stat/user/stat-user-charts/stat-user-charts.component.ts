import { combineLatest } from 'rxjs';
import { DateTime } from './../../../../model/date-time';
import { RfidHttpClient } from './../../../../service/rfid-http-client';
import { Component, OnInit, Input, SimpleChanges, OnChanges } from '@angular/core';
import { StatUser } from 'src/app/model/stat-user';
import { ModelFactory } from 'src/app/model/model-factory';

@Component({
  selector: 'app-stat-user-charts',
  templateUrl: './stat-user-charts.component.html',
  styleUrls: ['./stat-user-charts.component.css']
})
export class StatUserChartsComponent implements OnChanges {

  @Input()
  public user: StatUser;
  public workHourNormPerDays: DateTime[] = [];
  public entranceExitTimePerDay: any[] = [];
  public workHourNormStartSearchDate: Date;
  public workHourNormEndSearchDate: Date;
  public entranceExitTimeStartDate: Date;
  public entractExitTimeEndDate: Date;

  constructor(private rfidHttpClient: RfidHttpClient) {
  }

  public loadNormWorkHours(startDate: Date, endDate: Date): void {
    this.rfidHttpClient.getWorkHourNormPerDayForUser(this.user.id, startDate, endDate)
      .subscribe(
        data => {
          const tmp = data.map(x => ModelFactory.dateTimeAsWorkHourNormChartJson(x));
          this.workHourNormPerDays = [];
          this.workHourNormPerDays = [...tmp];
        },
        error => console.log(error)
      );
  }

  public loadEntranceExitTime(startDate: Date, endDate: Date) {
    combineLatest(
      this.rfidHttpClient.getEntranceTimeForUser(this.user.id, startDate, endDate),
      this.rfidHttpClient.getExitTimeForUser(this.user.id, startDate, endDate)
    ).subscribe(
      ([entrance, exit]) => {
        this.entranceExitTimePerDay = [];
        this.entranceExitTimePerDay.push({
          'name': 'Entrance',
          'series': entrance.map(x => ModelFactory.dateTimeAsEntranceExitChartJson(x))
        }, {
          'name': 'Exit',
          'series': exit.map(x => ModelFactory.dateTimeAsEntranceExitChartJson(x))
        });
      }, error => console.log(error));
  }

  public formatMinutesAsTime(minutes: number) {
    const hours = Math.floor(minutes / 60);
    const leftMinutes = minutes - hours * 60;
    return `${hours}:${leftMinutes}`;
  }

  public ngOnChanges(changes: SimpleChanges): void {
    this.setWorkNormSearchDates();
    this.setEntranceExitTimeSearchDates();
    this.loadNormWorkHours(this.workHourNormStartSearchDate, this.workHourNormEndSearchDate);
    this.loadEntranceExitTime(this.entranceExitTimeStartDate, this.entractExitTimeEndDate);
  }

  private setWorkNormSearchDates(): void {
    const normStartDate = new Date(Date.now());
    normStartDate.setMonth(normStartDate.getMonth() - 1);
    this.workHourNormStartSearchDate = normStartDate;
    this.workHourNormEndSearchDate = new Date(Date.now());
  }

  private setEntranceExitTimeSearchDates() {
    const startDate = new Date(Date.now());
    startDate.setMonth(startDate.getMonth() - 1);
    this.entranceExitTimeStartDate = startDate;
    this.entractExitTimeEndDate = new Date(Date.now());
  }
}

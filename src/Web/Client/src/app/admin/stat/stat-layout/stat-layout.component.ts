import { NavigationService } from './../../../service/navigation-service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-stat-layout',
  templateUrl: './stat-layout.component.html',
  styleUrls: ['./stat-layout.component.css']
})
export class StatLayoutComponent implements OnInit {

  constructor(public navigationService: NavigationService) { }

  ngOnInit() {
  }

}

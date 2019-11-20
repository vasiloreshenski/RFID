import { NavigationService } from './../../service/navigation-service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.css']
})
export class LayoutComponent implements OnInit {

  constructor(public navigationService: NavigationService) { }

  ngOnInit() {
  }

}

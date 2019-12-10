import { ProgressService } from './../../service/progress-service';
import { NavigationService } from './../../service/navigation-service';
import { Component, OnInit, HostBinding } from '@angular/core';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.css']
})
export class LayoutComponent implements OnInit {
  @HostBinding('id') id = 'app-layout';
  constructor(public navigationService: NavigationService, private progressService: ProgressService) { }

  public Title: String;
  public isLoading = true;

  ngOnInit() {
    this.navigationService.NavigatedEvent.subscribe(path => this.setTitleFromPath(path));
    // for cases when the user navigates direclty to the page
    this.setTitleFromPath(this.navigationService.currentPath());
    this.progressService.loading$.subscribe(value => {
      this.isLoading = value;
      console.log('is loading: ' + value);
    });
  }

  private setTitleFromPath(path: String): void {
    if (path.includes(NavigationService.StatRoute)) {
      this.Title = 'Statistics';
    } else if (path === NavigationService.AccessPointManagmentRoute) {
      this.Title = 'Access point managment';
    } else if (path === NavigationService.TagManagmentRoute) {
      this.Title = 'Tag managment';
    }
  }
}

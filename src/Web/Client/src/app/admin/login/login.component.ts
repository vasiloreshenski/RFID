import { NavigationService } from './../../service/navigation-service';
import { Component, OnInit, HostBinding } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/service/auth-service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  @HostBinding('class') class = 'h-center v-center';
  constructor(private authService: AuthService, private navigationService: NavigationService) { }

  login(email: string, password: string) {
    const authToken$ = this.authService.issueToken(email, password);
    authToken$.subscribe(
      token => {
        if (this.authService.isAuthenticated()) {
          this.navigationService.gotoStat();
        }
      },
      err => console.log(err)
    );
  }

  ngOnInit() {
  }

}

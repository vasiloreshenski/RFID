import { BearerHeaderHttpInterceptor } from './interceptor/bearer-header-http-interceptor';
import { UnAuthorizedHttpInterceptor } from './interceptor/un-authorized-http-interceptor';
import { AuthService } from 'src/app/service/auth-service';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA, ErrorHandler } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatSliderModule } from '@angular/material/slider';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatTableModule } from '@angular/material/table';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSnackBarModule, MAT_SNACK_BAR_DEFAULT_OPTIONS } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatPaginatorModule } from '@angular/material/paginator';

import { AppComponent } from './app.component';
import { LoginComponent } from './admin/login/login.component';
import { HashLocationStrategy, LocationStrategy, DatePipe } from '@angular/common';
import { RfidHttpClient } from './service/rfid-http-client';
import { HttpModule } from '@angular/http';
import { LayoutComponent } from './admin/layout/layout.component';
import { AccessPointListComponent } from './admin/access-point/access-point-list/access-point-list.component';
import { AuthGuard } from './guard/auth-guard';
import { NavigationService } from './service/navigation-service';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { AccessPointItemComponent } from './admin/access-point/access-point-item/access-point-item.component';
import { UnknownAccessPointItemComponent } from './admin/access-point/unknown-access-point-item/unknown-access-point-item.component';
import { TagListComponent } from './admin/tag/tag-list/tag-list.component';
import { TagItemComponent } from './admin/tag/tag-item/tag-item.component';
import { HtmlService } from './service/html-service';
import { UnknownTagItemComponent } from './admin/tag/unknown-tag-item/unknown-tag-item.component';
import { StatLayoutComponent } from './admin/stat/stat-layout/stat-layout.component';
import { StatUserLayoutComponent } from './admin/stat/user/stat-user-layout/stat-user-layout.component';
import { StatSummaryLayoutComponent } from './admin/stat/summary/stat-summary-layout/stat-summary-layout.component';
import { StatUserOverviewComponent } from './admin/stat/user/stat-user-overview/stat-user-overview.component';
import { StatUserListComponent } from './admin/stat/user/stat-user-list/stat-user-list.component';
import { StatUserChartsComponent } from './admin/stat/user/stat-user-charts/stat-user-charts.component';
import { NotificationService } from './service/notification-service';
import { GlobalErrorHandler } from './error/global-error-handler';
import { ProgressService } from './service/progress-service';

const routes: Routes = [
  { path: '', redirectTo: 'admin/login', pathMatch: 'full' },
  { path: 'admin/login', component: LoginComponent },
  {
    path: 'admin', component: LayoutComponent, canActivate: [AuthGuard], children: [{
      path: 'stat', component: StatLayoutComponent, children: [{
        path: '', redirectTo: 'user', pathMatch: 'full'
      }, {
        path: 'summary', component: StatSummaryLayoutComponent
      }, {
        path: 'user', component: StatUserLayoutComponent
      }]
    }, {
      path: 'accesspoint', component: AccessPointListComponent
    }, {
      path: 'tag', component: TagListComponent
    }]
  }
];

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    LayoutComponent,
    AccessPointListComponent,
    AccessPointItemComponent,
    UnknownAccessPointItemComponent,
    TagListComponent,
    TagItemComponent,
    UnknownTagItemComponent,
    StatLayoutComponent,
    StatUserLayoutComponent,
    StatSummaryLayoutComponent,
    StatUserOverviewComponent,
    StatUserListComponent,
    StatUserChartsComponent
  ],
  imports: [
    BrowserModule,
    RouterModule.forRoot(routes),
    HttpModule,
    HttpClientModule,
    NgxChartsModule,
    BrowserAnimationsModule,
    MatSliderModule,
    MatButtonModule,
    MatInputModule,
    MatSidenavModule,
    MatCardModule,
    MatSelectModule,
    FormsModule,
    ReactiveFormsModule,
    MatAutocompleteModule,
    MatTableModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    MatPaginatorModule
  ],
  providers: [
    { provide: LocationStrategy, useClass: HashLocationStrategy },
    { provide: HTTP_INTERCEPTORS, useClass: UnAuthorizedHttpInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: BearerHeaderHttpInterceptor, multi: true },
    { provide: MAT_SNACK_BAR_DEFAULT_OPTIONS, useValue: { duration: 2500, horizontalPosition: 'left', verticalPosition: 'top' } },
    { provide: ErrorHandler, useClass: GlobalErrorHandler },
    RfidHttpClient,
    AuthService,
    AuthGuard,
    NavigationService,
    HtmlService,
    DatePipe,
    NotificationService,
    ProgressService
  ],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule { }

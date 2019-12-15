import { AccessPoint } from './../model/access-point';
import { Pagination } from './../model/pagination';
import { environment } from './../../environments/environment';
import { StatUserOverview } from './../model/stat-user-overview';
import { RegisterTagRequestModel } from './../model/register-tag-request-model';
import { UnknownTag } from './../model/unknown-tag';
import { UpdateTagRequestModel } from './../model/update-tag-request-model';
import { UnknownAccessPoint } from './../model/unknown-access-point';
import { UpdateAccessPointRequestModel } from './../model/update-access-point-request-model';
import { ModelFactory } from '../model/model-factory';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse, HttpHeaders } from '@angular/common/http';
import { AccessLevelType } from '../model/access-level-type';
import { RegisterAccessPointRequestModel } from '../model/register-access-point-request-model';
import { Tag } from '../model/tag';
import { TagUser } from '../model/tag-user';
import { StatUser } from '../model/stat-user';
import { DatePipe } from '@angular/common';
import { DateTime } from '../model/date-time';

export class AuthTokenResponseModel {
    public token: string;
    public expireInSeconds: number;
    public refreshToken: string;
}

@Injectable()
export class RfidHttpClient {
    public API_URL = environment.apiUrl;

    constructor(private http: HttpClient, private datePipe: DatePipe) {
    }

    static parseAuthTokenResponse(response: Response): AuthTokenResponseModel {
        const json: any = response.json();
        const result = new AuthTokenResponseModel();
        result.token = json.token;
        result.expireInSeconds = json.expireInSeconds;
        result.refreshToken = json.refreshToken;

        return result;
    }

    public generateAuthToken(email: string, password: string): Observable<AuthTokenResponseModel> {
        const url = new URL('/auth/api/token/generate', this.API_URL).href;
        return this.http.post<AuthTokenResponseModel>(url, { email: email, password: password });
    }

    public getActiveAccessPoints(page: number, pageSize: number): Observable<Pagination<AccessPoint>> {
        const url = new URL(`/administration/api/accesspoint/active?page=${page}&pageSize=${pageSize}`, this.API_URL).href;
        const result$ = this.http.get<any>(url).pipe(
            map(json => ModelFactory.paginationFromJson(json, ModelFactory.accessPointFromJson))
        );

        return result$;
    }

    public getInActiveAccessPoints(page: number, pageSize: number): Observable<Pagination<AccessPoint>> {
        const url = new URL(`/administration/api/accesspoint/inactive?page=${page}&pageSize=${pageSize}`, this.API_URL).href;
        const result$ = this.http.get<any>(url).pipe(
            map(json => ModelFactory.paginationFromJson(json, ModelFactory.accessPointFromJson))
        );

        return result$;
    }

    public getDeletedAccessPoints(page: number, pageSize: number): Observable<Pagination<AccessPoint>> {
        const url = new URL(`/administration/api/accesspoint/deleted?page=${page}&pageSize=${pageSize}`, this.API_URL).href;
        const result$ = this.http.get<any>(url).pipe(
            map(json => ModelFactory.paginationFromJson(json, ModelFactory.accessPointFromJson))
        );

        return result$;
    }

    public getUnknownAccessPoints(page: number, pageSize: number): Observable<Pagination<UnknownAccessPoint>> {
        const url = new URL(`/administration/api/accesspoint/unknown?page=${page}&pageSize=${pageSize}`, this.API_URL).href;
        const result$ = this.http.get<any>(url).pipe(
            map(json => ModelFactory.paginationFromJson(json, ModelFactory.unknownAccessPointFromJson))
        );

        return result$;
    }

    public getActiveTags(): Observable<Tag[]> {
        const url = new URL('/administration/api/tags/active', this.API_URL).href;
        const result$ = this.http.get<Tag[]>(url).pipe(
            map(data => data.map(json => ModelFactory.tagFromJson(json)))
        );

        return result$;
    }

    public getInActiveTags(): Observable<Tag[]> {
        const url = new URL('/administration/api/tags/inactive', this.API_URL).href;
        const result$ = this.http.get<Tag[]>(url).pipe(
            map(data => data.map(json => ModelFactory.tagFromJson(json)))
        );
        return result$;
    }

    public getDeletedTags(): Observable<Tag[]> {
        const url = new URL('/administration/api/tags/deleted', this.API_URL).href;
        const result$ = this.http.get<Tag[]>(url).pipe(
            map(data => data.map(json => ModelFactory.tagFromJson(json)))
        );
        return result$;
    }

    public getTagsUsers(): Observable<TagUser[]> {
        const url = new URL('/administration/api/tags/users', this.API_URL).href;
        const result$ = this.http.get<Tag[]>(url).pipe(
            map(data => data.map(json => ModelFactory.tagUserFromJson(json)))
        );
        return result$;
    }

    public getUnknownTags(): Observable<UnknownTag[]> {
        const url = new URL('/administration/api/tags/unknown', this.API_URL).href;
        const result$ = this.http.get<Tag[]>(url).pipe(
            map(data => data.map(json => ModelFactory.unknownTagFromJson(json)))
        );
        return result$;
    }

    public getStatUserOverview(): Observable<StatUserOverview> {
        const url = new URL('/stat/api/users/overview', this.API_URL).href;
        const result$ = this.http.get<StatUserOverview>(url).pipe(
            map(data => ModelFactory.statUserOverviewFromJson(data))
        );
        return result$;
    }

    public getStatUsers(): Observable<StatUser[]> {
        const url = new URL('/stat/api/users/all', this.API_URL).href;
        const result$ = this.http.get<StatUser[]>(url).pipe(
            map(data => data.map(json => ModelFactory.statUserFromJson(json)))
        );
        return result$;
    }

    public getWorkHourNormPerDayForUser(userId: number, startDate: Date, endDate: Date): Observable<DateTime[]> {
        let url = new URL(`/stat/api/users/norm?userId=${encodeURIComponent(userId)}`, this.API_URL).href;
        if (startDate) {
            url = `${url}&startDate=${encodeURIComponent(this.datePipe.transform(startDate, 'MM/dd/yyyy'))}`;
        }
        if (endDate) {
            url = `${url}&endDate=${encodeURIComponent(this.datePipe.transform(endDate, 'MM/dd/yyyy'))}`;
        }
        const result$ = this.http.get<DateTime[]>(url).pipe(
            map(data => {
                return data.map(json => ModelFactory.dateTimeFromJson(json));
            })
        );
        return result$;
    }

    public getEntranceTimeForUser(userId: number, startDate: Date, endDate: Date): Observable<DateTime[]> {
        let url = new URL(`/stat/api/users/entrance?userId=${encodeURIComponent(userId)}`, this.API_URL).href;
        if (startDate) {
            url = `${url}&startDate=${encodeURIComponent(this.datePipe.transform(startDate, 'MM/dd/yyyy'))}`;
        }
        if (endDate) {
            url = `${url}&endDate=${encodeURIComponent(this.datePipe.transform(endDate, 'MM/dd/yyyy'))}`;
        }

        const result$ = this.http.get<DateTime[]>(url).pipe(
            map(data => data.map(json => ModelFactory.dateTimeFromJson(json)))
        );
        return result$;
    }

    public getExitTimeForUser(userId: number, startDate: Date, endDate: Date): Observable<DateTime[]> {
        let url = new URL(`/stat/api/users/exit?userId=${encodeURIComponent(userId)}`, this.API_URL).href;
        if (startDate) {
            url = `${url}&startDate=${encodeURIComponent(this.datePipe.transform(startDate, 'MM/dd/yyyy'))}`;
        }
        if (endDate) {
            url = `${url}&endDate=${encodeURIComponent(this.datePipe.transform(endDate, 'MM/dd/yyyy'))}`;
        }

        const result$ = this.http.get<DateTime[]>(url).pipe(
            map(data => data.map(json => ModelFactory.dateTimeFromJson(json)))
        );
        return result$;
    }


    public updateAccessPoint(accessPoint: UpdateAccessPointRequestModel): Observable<HttpResponse<Object>> {
        const url = new URL('/administration/api/accesspoint/update', this.API_URL).href;
        return this.http.patch(url, accessPoint, { observe: 'response', headers: new HttpHeaders({ 'Content-Type': 'application/json' }) });
    }

    public registerAccessPoint(accessPoint: RegisterAccessPointRequestModel): Observable<HttpResponse<Object>> {
        const url = new URL('/administration/api/accesspoint/register', this.API_URL).href;
        return this.http.post(url, accessPoint, { observe: 'response', headers: new HttpHeaders({ 'Content-Type': 'application/json' }) });
    }

    public activateAccessPoint(id: number): Observable<HttpResponse<Object>> {
        const url = new URL('/administration/api/accesspoint/activate', this.API_URL).href;
        return this.http.patch(url, { id: id }, { observe: 'response', headers: new HttpHeaders({ 'Content-Type': 'application/json' }) });
    }

    public deActivateAccessPoint(id: number): Observable<HttpResponse<Object>> {
        const url = new URL('/administration/api/accesspoint/deactivate', this.API_URL).href;
        return this.http.patch(url, { id: id }, { observe: 'response', headers: new HttpHeaders({ 'Content-Type': 'application/json' }) });
    }

    public deleteAccessPoint(id: number): Observable<HttpResponse<Object>> {
        const url = new URL('/administration/api/accesspoint/delete', this.API_URL).href;
        return this.http.patch(url, { id: id }, { observe: 'response', headers: new HttpHeaders({ 'Content-Type': 'application/json' }) });
    }

    public unDeleteAccessPoint(id: number): Observable<HttpResponse<Object>> {
        const url = new URL('/administration/api/accesspoint/undelete', this.API_URL).href;
        return this.http.patch(url, { id: id }, { observe: 'response', headers: new HttpHeaders({ 'Content-Type': 'application/json' }) });
    }

    public registerTag(tag: RegisterTagRequestModel): Observable<HttpResponse<Object>> {
        const url = new URL('/administration/api/tags/register', this.API_URL).href;
        const result$ = this.http.post(url, tag, { observe: 'response', headers: new HttpHeaders({ 'Content-Type': 'application/json' }) });
        return result$;
    }

    public updateTag(requestModel: UpdateTagRequestModel): Observable<HttpResponse<Object>> {
        const url = new URL('/administration/api/tags/update', this.API_URL).href;
        const result$ = this.http.patch(url, requestModel, { observe: 'response', headers: new HttpHeaders({ 'Content-Type': 'application/json' }) });
        return result$;
    }

    public activateTag(id: number): Observable<HttpResponse<Object>> {
        const url = new URL('/administration/api/tags/activate', this.API_URL).href;
        const result$ = this.http.patch(url, { id: id }, { observe: 'response', headers: new HttpHeaders({ 'Content-Type': 'application/json' }) });
        return result$;
    }

    public deActivateTag(id: number): Observable<HttpResponse<Object>> {
        const url = new URL('/administration/api/tags/deactivate', this.API_URL).href;
        const result$ = this.http.patch(url, { id: id }, { observe: 'response', headers: new HttpHeaders({ 'Content-Type': 'application/json' }) });
        return result$;
    }

    public deleteTag(id: number): Observable<HttpResponse<Object>> {
        const url = new URL('/administration/api/tags/delete', this.API_URL).href;
        const result$ = this.http.patch(url, { id: id }, { observe: 'response', headers: new HttpHeaders({ 'Content-Type': 'application/json' }) });
        return result$;
    }

    public unDeleteTag(id: number): Observable<HttpResponse<Object>> {
        const url = new URL('/administration/api/tags/undelete', this.API_URL).href;
        const result$ = this.http.patch(url, { id: id }, { observe: 'response', headers: new HttpHeaders({ 'Content-Type': 'application/json' }) });
        return result$;
    }

    public logClientError(message: String): Observable<HttpResponse<Object>> {
        const url = new URL('/log/api/client/error', this.API_URL).href;
        const result$ = this.http.post(url, { message: message }, { observe: 'response', headers: new HttpHeaders({ 'Content-Type': 'application/json' }) });
        return result$;
    }
}

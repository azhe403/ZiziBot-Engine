import {HttpClient} from "@angular/common/http";
import {Injectable} from '@angular/core';
import {AboutApi} from "projects/zizibot-types/src/restapi/about-api";
import {ApiResponse} from "projects/zizibot-types/src/restapi/api-response";

@Injectable({
    providedIn: 'root'
})
export class AboutService {

    constructor(private http: HttpClient) {
    }

    public getAbout() {
        return this.http.get<ApiResponse<AboutApi>>("/api");
    }
}

import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ApiUrl} from "../../../../projects/zizibot-types/src/constant/api-url";
import {WelcomeMessage} from "../../../../projects/zizibot-types/src/restapi/welcome-message";
import {ApiResponse} from "../../../../projects/zizibot-types/src/restapi/api-response";

@Injectable({
    providedIn: 'root'
})
export class GroupService {

    constructor(private http: HttpClient) {
    }

    public getWelcomeMessage() {
        return this.http.get<ApiResponse<WelcomeMessage[]>>(ApiUrl.API_WELCOME_MESSAGE);
    }

    public selectWelcomeMessage(welcomeMessageId: string) {
        return this.http.post<ApiResponse<object[]>>(ApiUrl.API_WELCOME_MESSAGE_SELECT, {
            welcomeId: welcomeMessageId
        });
    }
}

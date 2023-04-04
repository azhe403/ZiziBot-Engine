import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ApiUrl} from "zizibot-contracts/dist/src/constant/api-url";
import {ApiResponse} from "zizibot-contracts/dist/src/restapi/api-response";
import {WelcomeMessage} from "zizibot-contracts/dist/src/restapi/welcome-message";

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

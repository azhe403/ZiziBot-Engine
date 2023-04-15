import {HttpClient} from "@angular/common/http";
import {Injectable} from '@angular/core';
import {Observable} from "rxjs";
import {ApiUrl} from "../../../../projects/zizibot-types/src/constant/api-url";
import {ApiResponse} from "../../../../projects/zizibot-types/src/restapi/api-response";
import {WelcomeMessage} from "../../../../projects/zizibot-types/src/restapi/welcome-message";

@Injectable({
    providedIn: 'root'
})
export class GroupService {

    constructor(private http: HttpClient) {
    }

    public getWelcomeMessage(chatId: number): Observable<ApiResponse<WelcomeMessage[]>> {
        return this.http.get<ApiResponse<WelcomeMessage[]>>(ApiUrl.API_WELCOME_MESSAGE, {
            params: {
                chatId: chatId
            }
        });
    }

    public getWelcomeMessageById(welcomeId: string | null | undefined): Observable<ApiResponse<WelcomeMessage>> {
        return this.http.get<ApiResponse<WelcomeMessage>>(ApiUrl.API_WELCOME_MESSAGE + '/' + welcomeId);
    }

    public saveWelcomeMessage(data: any): Observable<ApiResponse<WelcomeMessage>> {
        return this.http.post<ApiResponse<WelcomeMessage>>(ApiUrl.API_WELCOME_MESSAGE, data);
    }

    public selectWelcomeMessage(data: any) {
        return this.http.post<ApiResponse<object[]>>(ApiUrl.API_WELCOME_MESSAGE_SELECT, data);
    }

    public deleteWelcomeMessage(data: any) {
        return this.http.delete<ApiResponse<WelcomeMessage>>(ApiUrl.API_WELCOME_MESSAGE, {
            body: data
        });
    }
}

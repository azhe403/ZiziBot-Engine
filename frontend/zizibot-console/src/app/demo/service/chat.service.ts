import {HttpClient} from "@angular/common/http";
import {Injectable} from '@angular/core';
import {ApiUrl} from "projects/zizibot-types/src/constant/api-url";
import {ApiResponse} from "projects/zizibot-types/src/restapi/api-response";
import {Note} from "projects/zizibot-types/src/restapi/note";

@Injectable({
    providedIn: 'root'
})
export class ChatService {

    constructor(private http: HttpClient) {
    }

    public getNote(chatId: number) {
        return this.http.get<ApiResponse<Note[]>>(ApiUrl.NOTE, {
            params: {
                chatId: chatId
            }
        });
    }
}

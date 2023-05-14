import {HttpClient} from "@angular/common/http";
import {Injectable} from '@angular/core';
import {ApiUrl} from "projects/zizibot-types/src/constant/api-url";
import {ApiResponse} from "projects/zizibot-types/src/restapi/api-response";
import {Note} from "projects/zizibot-types/src/restapi/note";
import {Rss} from "../../../../projects/zizibot-types/src/restapi/rss";

@Injectable({
    providedIn: 'root'
})
export class ChatService {

    constructor(private http: HttpClient) {
    }

    public saveNote(data: any) {
        return this.http.post<ApiResponse<Note>>(ApiUrl.NOTE, data);
    }

    public getNote(chatId: number) {
        return this.http.get<ApiResponse<Note[]>>(ApiUrl.NOTE, {
            params: {
                chatId: chatId
            }
        });
    }

    public getNoteById(noteId: string | null | undefined) {
        return this.http.get<ApiResponse<Note>>(ApiUrl.NOTE + '/' + noteId);
    }

    public deleteNote(data: any) {
        return this.http.delete<ApiResponse<Note>>(ApiUrl.NOTE, {
            body: data
        });
    }

    public getListRss(chatId: number) {
        return this.http.get<ApiResponse<Rss[]>>(ApiUrl.RSS, {
            params: {
                chatId: chatId
            }
        });
    }
}

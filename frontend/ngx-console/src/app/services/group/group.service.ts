import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ApiResponse} from "../../types/api-response";
import {WelcomeMessage} from "../../types/welcome-message";

@Injectable({
  providedIn: 'root'
})
export class GroupService {

  constructor(private httpClient: HttpClient) {
  }

  public getWelcomeMessages(chatId: number) {
    return this.httpClient.get<ApiResponse<WelcomeMessage[]>>('/api/group/welcome-message');
  }
}
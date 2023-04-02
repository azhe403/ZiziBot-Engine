import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ApiUrl} from "../../consts/api-url";
import {TelegramGroup} from "../../types/telegram-group";
import {ApiResponse} from "../../types/api-response";

@Injectable({
  providedIn: 'root'
})
export class TelegramService {

  constructor(
    private httpClient: HttpClient
  ) {
  }

  public listTelegramGroup() {
    return this.httpClient.get<ApiResponse<TelegramGroup[]>>(ApiUrl.TELEGRAM_LIST_GROUP);
  }
}
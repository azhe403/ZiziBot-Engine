import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ApiResponse} from 'projects/zizibot-types/src/restapi/api-response';
import {TelegramGroup} from 'projects/zizibot-types/src/restapi/telegram-group';
import {ApiUrl} from 'projects/zizibot-types/src/constant/api-url';

@Injectable()
export class TelegramService {

    constructor(
        private httpClient: HttpClient
    ) {
    }

    public listTelegramGroup() {
        return this.httpClient.get<ApiResponse<TelegramGroup[]>>(ApiUrl.TELEGRAM_LIST_GROUP);
    }
}

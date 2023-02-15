import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import * as uuid from "uuid";
import {firstValueFrom, map, Observable} from "rxjs";
import {TelegramUserLogin} from "../../types/TelegramUserLogin";
import {ApiResponse} from "../../types/api-response";
import {ApiUrl} from "../../consts/api-url";
import {DashboardSession} from "../../types/dashboard-session";
import {StorageService} from "../storage/storage.service";

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  constructor(private storageService: StorageService, private httpClient: HttpClient) {
  }

  public checkSessionId(sessionId: string): Promise<DashboardSession> {
    if (sessionId) {
      const result = this.httpClient.post<ApiResponse<DashboardSession>>(ApiUrl.SESSION_VALIDATE_ID,
        {
          sessionId: sessionId
        }, {
          headers: {
            'transactionId': uuid.v4()
          }
        })
        .pipe(map(x => {
          return x.result;
        }));

      return firstValueFrom(result);
    }

    return {} as Promise<DashboardSession>;
  }

  public async checkSession(): Promise<DashboardSession> {
    const userId = this.storageService.get('user_id');
    const sessionId = this.storageService.get('session_id');

    if (userId || sessionId) {
      const result = this.httpClient.post<ApiResponse<DashboardSession>>(ApiUrl.SESSION_VALIDATE,
        {
          userId: userId,
          sessionId: sessionId
        }, {
          headers: {
            'transactionId': uuid.v4()
          }
        })
        .pipe(map(x => {
          return x.result;
        }));

      return firstValueFrom(result);
    }

    return {} as DashboardSession;
  }

  public async saveSession(userLogin: TelegramUserLogin): Promise<boolean> {
    console.debug('sending user session');
    const sessionId = userLogin.sessionId;

    this.storageService.set("session_id", sessionId);

    const result = await this.httpClient.post<ApiResponse<boolean>>(ApiUrl.SESSION_SAVE,
      {
        id: userLogin.id,
        first_name: userLogin.first_name,
        username: userLogin.username,
        photo_url: userLogin.photo_url,
        hash: userLogin.hash,
        session_id: sessionId
      }, {
        headers: {
          'transactionId': uuid.v4()
        }
      }).pipe(map(x => {
      return x.result;
    }));

    return firstValueFrom(result)
  }

  public logoutSession() {
    console.debug('Clearing dashboard session..')
    this.storageService.delete('session_id');
    this.storageService.delete('user_id');
  }
}
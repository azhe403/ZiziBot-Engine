import {Injectable} from '@angular/core';
import {CookieService} from "ngx-cookie-service";
import {HttpClient} from "@angular/common/http";
import * as uuid from "uuid";
import {firstValueFrom, map} from "rxjs";
import {TelegramUserLogin} from "../../types/TelegramUserLogin";
import {ApiResponse} from "../../types/api-response";

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  constructor(private cookieService: CookieService, private httpClient: HttpClient) {
  }

  public async checkSession(): Promise<boolean> {
    const userId = this.cookieService.get('user_id');
    const sessionId = this.cookieService.get('session_id');

    if (userId || sessionId) {
      const result = await this.httpClient.post<ApiResponse<boolean>>('/api/user/session/telegram/validate',
        {
          userId: userId,
          sessionId: sessionId
        })

        .pipe(map(x => {
          return x.result;
        }));

      return firstValueFrom(result);
    }

    return false;
  }

  public saveSession(userLogin: TelegramUserLogin) {
    console.log('sending user session');
    const sessionId = uuid.v4();

    this.cookieService.set("session_id", sessionId);
    this.cookieService.set("user_id", userLogin.id.toString());

    this.httpClient.post('/api/user/session/telegram', {
      id: userLogin.id,
      first_name: userLogin.first_name,
      username: userLogin.username,
      photo_url: userLogin.photo_url,
      hash: userLogin.hash,
      session_id: sessionId
    })
      .subscribe(value => {
      });
  }

  public logoutSession() {
    console.debug('Clearing dashboard session..')
    this.cookieService.delete('session_id');
    this.cookieService.delete('user_id');
  }
}

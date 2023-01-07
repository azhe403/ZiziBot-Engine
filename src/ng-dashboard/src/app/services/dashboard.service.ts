import {Injectable} from '@angular/core';
import {CookieService} from "ngx-cookie-service";
import {HttpClient} from "@angular/common/http";
import {TelegramUserLogin} from "../types/TelegramUserLogin";
import * as uuid from "uuid";

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  constructor(private cookieService: CookieService, private httpClient: HttpClient) {
  }

  public checkCookie() {
    // if (ssid && token) {
    //   return true;
    // }

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
}

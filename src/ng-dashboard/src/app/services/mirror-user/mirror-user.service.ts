import {Injectable} from '@angular/core';
import {CookieService} from "ngx-cookie-service";
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {MirrorUser} from "../../types/mirror-user";

@Injectable({
  providedIn: 'root'
})
export class MirrorUserService {

  constructor(
    private cookieService: CookieService,
    private httpClient: HttpClient
  ) {
  }

  public getUsers(): Observable<MirrorUser[]> {
    return this.httpClient.get<MirrorUser[]>('/api/mirror-user');
  }
}

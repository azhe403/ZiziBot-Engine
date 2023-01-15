import {Injectable} from '@angular/core';
import {CookieService} from "ngx-cookie-service";
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {AddMirrorUserDto, MirrorUser} from "../../types/mirror-user";
import {ApiResponse} from "../../types/api-response";
import {ApiUrl} from "../../consts/api-url";

@Injectable({
  providedIn: 'root'
})
export class MirrorUserService {

  constructor(
    private cookieService: CookieService,
    private httpClient: HttpClient
  ) {
  }

  public getUsers(): Observable<ApiResponse<MirrorUser[]>> {
    return this.httpClient.get<ApiResponse<MirrorUser[]>>(ApiUrl.MIRROR_USERS);
  }

  public saveUser(userDto: AddMirrorUserDto): Observable<any> {
    return this.httpClient.post(ApiUrl.MIRROR_USERS, userDto);
  }

  public deleteUser(userId: number): Observable<any> {
    return this.httpClient.delete(ApiUrl.MIRROR_USERS, {
      params:
        {
          userId: userId
        }
    });
  }
}

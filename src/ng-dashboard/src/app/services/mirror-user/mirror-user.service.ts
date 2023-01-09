import {Injectable} from '@angular/core';
import {CookieService} from "ngx-cookie-service";
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {AddMirrorUserDto, MirrorUser} from "../../types/mirror-user";

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

  public saveUser(userDto: AddMirrorUserDto): Observable<any> {
    return this.httpClient.post('/api/mirror-user', userDto);
  }

  public deleteUser(userId: number): Observable<any> {
    return this.httpClient.delete(`/api/mirror-user`, {
      params:
        {
          userId: userId
        }
    });
  }
}

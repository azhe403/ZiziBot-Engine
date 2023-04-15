import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {AddMirrorUserDto, MirrorUser} from "../../types/mirror-user";
import {ApiResponse} from "../../types/api-response";
import {ApiUrl} from "../../consts/api-url";
import {StorageService} from "../storage/storage.service";

@Injectable({
  providedIn: 'root'
})
export class MirrorUserService {

  constructor(
    private storageService: StorageService,
    private httpClient: HttpClient
  ) {
  }

  public getUsers(): Observable<ApiResponse<MirrorUser[]>> {
    return this.httpClient.get<ApiResponse<MirrorUser[]>>(ApiUrl.MIRROR_USERS);
  }

  public getUser(userId: number): Observable<ApiResponse<MirrorUser>> {
    return this.httpClient.get<ApiResponse<MirrorUser>>(ApiUrl.FIND_MIRROR_USER, {
      params: {
        userId: userId
      }
    });
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
import {Injectable} from '@angular/core';
import {StorageService} from '../storage/storage.service';
import {HttpClient} from '@angular/common/http';
import {ApiUrl} from '../../consts/api-url';
import {Observable} from 'rxjs';
import * as uuid from "uuid";
import {ApiResponse} from '../../types/api-response';
import {Antispam} from '../../types/antispam';

@Injectable({
  providedIn: 'root'
})
export class AntispamService {

  constructor(private storageService: StorageService, private httpClient: HttpClient) {
  }

  public getFedBanList(): Observable<ApiResponse<Antispam[]>> {
    return this.httpClient.get<ApiResponse<Antispam[]>>(ApiUrl.ANTI_SPAM,
      {
        headers: {
          'transactionId': uuid.v4()
        }
      });
  }

  public saveBan(body: any) {
    return this.httpClient.post(ApiUrl.ANTI_SPAM,
      body,
      {
        headers: {
          'transactionId': uuid.v4()
        }
      });
  }

  deleteFedBan(id: number) {
    return this.httpClient.delete(ApiUrl.ANTI_SPAM,
      {
        body: {
          userId: id
        },
        headers: {
          'transactionId': uuid.v4()
        }
      });
  }
}
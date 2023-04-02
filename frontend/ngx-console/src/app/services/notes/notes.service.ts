import {Injectable} from '@angular/core';
import {StorageService} from '../storage/storage.service';
import {HttpClient} from '@angular/common/http';
import {ApiUrl} from '../../consts/api-url';
import {ApiResponse} from '../../types/api-response';
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NotesService {

  constructor(
    private storageService: StorageService,
    private httpClient: HttpClient
  ) {
  }

  public saveNote(body: any): Observable<ApiResponse<any>> {
    return this.httpClient.post<ApiResponse<any>>(ApiUrl.TAG, body);
  }

}
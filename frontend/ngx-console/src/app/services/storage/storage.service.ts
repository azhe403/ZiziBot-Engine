import {Injectable} from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class StorageService {

  constructor() {
  }

  public set(key: string, data: string) {
    localStorage.setItem(key, data);
  }

  public get(key: string): string {
    return localStorage.getItem(key) ?? "";
  }

  public delete(key: string) {
    localStorage.removeItem(key);
  }

  public clear() {
    localStorage.clear();
  }
}

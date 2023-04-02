import {Injectable} from '@angular/core';
import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {Observable} from 'rxjs';
import * as uuid from "uuid";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor() {
  }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const bearerToken = localStorage.getItem('bearer_token');

    if (bearerToken) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${bearerToken}`
        }
      });
    }

    request = request.clone({
      setHeaders: {
        'Content-Type': 'application/json',
        'transactionId': uuid.v4()
      },
      responseType: 'json'
    });

    return next.handle(request);
  }
}
import {HTTP_INTERCEPTORS} from "@angular/common/http";
import {GlobalHttpInterceptor} from "./global-http.interceptor.service";

export const HttpInterceptorsService = [
    {provide: HTTP_INTERCEPTORS, useClass: GlobalHttpInterceptor, multi: true},
];

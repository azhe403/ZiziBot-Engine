import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {firstValueFrom, map} from "rxjs";
import {TelegramUserLogin} from "../../types/TelegramUserLogin";
import {ApiResponse} from "../../types/api-response";
import {ApiUrl} from "../../consts/api-url";
import {DashboardSession} from "../../types/dashboard-session";
import {StorageService} from "../storage/storage.service";
import {CookieService} from "ngx-cookie-service";

@Injectable({
    providedIn: 'root'
})
export class DashboardService {

    constructor(
        private storageService: StorageService,
        private cookieService: CookieService,
        private httpClient: HttpClient
    ) {
    }

    public checkBearerSession(): Promise<DashboardSession> {
        const result = this.httpClient.post<ApiResponse<DashboardSession>>(ApiUrl.SESSION_VALIDATE_ID, {})
            .pipe(map(x => {
                return x.result;
            }));

        return firstValueFrom(result);

    }

    public async checkSession(): Promise<DashboardSession> {
        const userId = this.storageService.get('user_id');
        const sessionId = this.storageService.get('session_id');

        if (userId || sessionId) {
            const result = this.httpClient.post<ApiResponse<DashboardSession>>(ApiUrl.SESSION_VALIDATE,
                {
                    userId: userId,
                    sessionId: sessionId
                })
                .pipe(map(x => {
                    return x.result;
                }));

            return firstValueFrom(result);
        }

        return {} as DashboardSession;
    }

    public async saveSession(userLogin: TelegramUserLogin): Promise<DashboardSession> {
        console.debug('sending user session');
        const sessionId = userLogin.sessionId;

        const dashboardSessionObservable = this.httpClient.post<ApiResponse<DashboardSession>>(ApiUrl.SESSION_SAVE, {
            id: userLogin.id,
            first_name: userLogin.first_name,
            username: userLogin.username,
            photo_url: userLogin.photo_url,
            hash: userLogin.hash,
            session_id: sessionId
        })
            .pipe(map(x => {
                return x.result;
            }));

        const session = await firstValueFrom(dashboardSessionObservable);
        console.debug('session saved', session);

        localStorage.setItem('bearer_token', session.bearerToken);
        this.cookieService.set(`bearer_token`, session.bearerToken, {
            path: '/hangfire-jobs',
        });

        return session;
    }

    public logoutSession() {
        console.debug('Clearing dashboard session..')
        this.storageService.delete('session_id');
        this.storageService.delete('user_id');
    }
}
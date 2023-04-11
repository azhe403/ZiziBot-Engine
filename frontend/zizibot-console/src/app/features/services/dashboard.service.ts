import {HttpClient} from "@angular/common/http";
import {Injectable} from '@angular/core';
import {CookieService} from "ngx-cookie-service";
import {StorageService} from "projects/zizibot-services/src/storage.service";
import {ApiUrl} from 'projects/zizibot-types/src/constant/api-url';
import {StorageKey} from "projects/zizibot-types/src/constant/storage-key";
import {ApiResponse} from 'projects/zizibot-types/src/restapi/api-response';
import {DashboardSession} from 'projects/zizibot-types/src/restapi/dashboard-session';
import {TelegramUserLogin} from 'projects/zizibot-types/src/restapi/user-login';
import {UserSession} from 'projects/zizibot-types/src/restapi/user-session';
import {firstValueFrom, map} from "rxjs";

@Injectable({providedIn: 'root'})
export class DashboardService {

    constructor(
        private storageService: StorageService,
        private cookieService: CookieService,
        private httpClient: HttpClient
    ) {
    }

    public checkBearerSession(): Promise<UserSession> {
        const result = this.httpClient.post<ApiResponse<UserSession>>(ApiUrl.SESSION_VALIDATE_ID, {})
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

        localStorage.setItem(StorageKey.BEARER_TOKEN, session.bearerToken);
        this.cookieService.set(StorageKey.BEARER_TOKEN, session.bearerToken, {
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

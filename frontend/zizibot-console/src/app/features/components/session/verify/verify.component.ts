import {AfterViewInit, Component, OnDestroy} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {StorageKey} from "projects/zizibot-types/src/constant/storage-key";
import {map, Subscription, timer} from "rxjs";
import {DashboardService} from "../../../services/dashboard.service";
import {TelegramUserLogin} from "zizibot-contracts/src/restapi/user-login";

@Component({
    selector: 'app-verify',
    templateUrl: './verify.component.html',
    styleUrls: ['./verify.component.scss']
})
export class VerifyComponent implements AfterViewInit, OnDestroy {
    authMessage: string[] = [];
    routeSubs: Subscription | undefined;

    constructor(
        private route: ActivatedRoute,
        private dashboardService: DashboardService,
        private router: Router
    ) {
    }

    ngOnDestroy(): void {
        this.routeSubs?.unsubscribe();
    }

    ngAfterViewInit(): void {
        this.verifySession();
    }

    private verifySession() {
        this.authMessage.push('Sedang masuk..');
        localStorage.removeItem(StorageKey.BEARER_TOKEN);

        this.routeSubs = this.route.queryParams
            .pipe(map(x => x as TelegramUserLogin))
            .subscribe({
                next: async value => {
                    console.debug('queryParams:', value);

                    try {
                        await this.dashboardService.saveSession(value);

                        this.authMessage.push('Anda berhasil masuk..');
                        timer(3000).subscribe(x => {
                            this.authMessage.push('Sedang mengalihkan..');
                            this.router.navigate(['/']).then(r => {
                                console.debug('after verify session', r);
                            });
                        });
                    } catch (err: any) {
                        console.debug('create session error', err);
                        this.authMessage.push(err.error.message)
                    }
                },
                error: err => {
                    console.debug('create session error', err);
                    this.authMessage.push(err.toString())
                },
                complete: () => {
                    console.debug('create session complete');
                }
            });
    }

}

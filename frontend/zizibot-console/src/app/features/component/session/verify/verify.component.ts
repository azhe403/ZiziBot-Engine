import {AfterViewInit, Component} from '@angular/core';
import {map, timer} from "rxjs";
import {ActivatedRoute, Router} from "@angular/router";
import {DashboardService} from "../../../services/dashboard.service";
import {TelegramUserLogin} from "../../../../../../projects/zizibot-types/src/restapi/user-login";

@Component({
    selector: 'app-verify',
    templateUrl: './verify.component.html',
    styleUrls: ['./verify.component.scss']
})
export class VerifyComponent implements AfterViewInit {
    authMessage: string[] = [];

    constructor(
        private route: ActivatedRoute,
        private dashboardService: DashboardService,
        private router: Router
    ) {
    }

    ngAfterViewInit(): void {
        this.verifySession();
    }

    private verifySession() {
        this.authMessage.push('Sedang masuk..');

        this.route.queryParams
            .pipe(map(x => x as TelegramUserLogin))
            .subscribe(async value => {
                console.debug('queryParams:', value);
                await this.dashboardService.saveSession(value);

                this.authMessage.push('Anda berhasil masuk..');
                timer(3000).subscribe(x => {
                    this.authMessage.push('Sedang mengalihkan..');
                    this.router.navigate(['/']).then(r => {
                        console.debug('after verify session', r);
                    });
                });
            });
    }

}

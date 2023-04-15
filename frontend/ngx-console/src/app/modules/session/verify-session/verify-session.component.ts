import {Component, OnDestroy, OnInit} from '@angular/core';
import {StorageService} from '../../../services/storage/storage.service';
import {map, Subscription, timer} from 'rxjs';
import {ActivatedRoute, Router} from '@angular/router';
import {DashboardService} from '../../../services/dashboard/dashboard.service';
import {CookieService} from 'ngx-cookie-service';
import {TelegramUserLogin} from '../../../types/TelegramUserLogin';

@Component({
  selector: 'app-verify-session',
  templateUrl: './verify-session.component.html',
  styleUrls: ['./verify-session.component.scss']
})
export class VerifySessionComponent implements OnInit, OnDestroy {

  routeSubs: Subscription;
  authMessage: string[] = [];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private storageService: StorageService,
    private dashboardService: DashboardService,
    private cookieService: CookieService
  ) {
    this.routeSubs = new Subscription();
  }

  ngOnInit(): void {
    this.authMessage.push('Sedang memverifikasi..');

    this.routeSubs = this.route.queryParams
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

  ngOnDestroy(): void {
    this.routeSubs.unsubscribe();
  }
}
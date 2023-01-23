import {Component, OnDestroy, OnInit} from '@angular/core';
import {StorageService} from '../../../services/storage/storage.service';
import {map, Subscription, timer} from 'rxjs';
import {ActivatedRoute, Router} from '@angular/router';
import {DashboardService} from '../../../services/dashboard/dashboard.service';
import {CookieService} from 'ngx-cookie-service';

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

    this.routeSubs = this.route.paramMap
      .pipe(map(x => x))
      .subscribe(async value => {
        const sessionId = value.get('sessionId') ?? "";
        console.debug('route:', sessionId)

        const session = await this.dashboardService.checkSessionId(sessionId);
        console.debug('check sessionId', session);

        if (session.isSessionValid) {
          this.storageService.set('session_id', sessionId);
          this.storageService.set('user_id', session.userId.toString());
          this.cookieService.set('session_id', sessionId, undefined, '/');

          this.authMessage.push('Anda berhasil masuk..');
        } else {
          this.authMessage.push('Sesi Anda tidak valid!')
        }

        timer(3000).subscribe(value1 => {
          this.authMessage.push('Sedang mengalihkan..')
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
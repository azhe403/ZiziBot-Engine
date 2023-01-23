import {Component, OnDestroy, OnInit} from '@angular/core';
import {StorageService} from '../../../services/storage/storage.service';
import {map, Subscription} from 'rxjs';
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
    this.routeSubs = this.route.paramMap
      .pipe(map(x => x))
      .subscribe(value => {
        const sessionId = value.get('sessionId') ?? "";
        console.debug('route:', sessionId)

        this.storageService.set('session_id', sessionId);
        this.cookieService.set('session_id', sessionId, undefined, '/');

        this.dashboardService.checkSessionId();
        this.router.navigate(['/']).then(r => {
          console.debug('after verify session', r);
          window.location.reload();
        });
      });
  }

  ngOnDestroy(): void {
    this.routeSubs.unsubscribe();
  }
}
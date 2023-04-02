import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {DashboardService} from "../services/dashboard/dashboard.service";
import {TelegramUserLogin} from "../types/TelegramUserLogin";
import {map} from "rxjs";

@Component({
  selector: 'app-after-telegram-login',
  templateUrl: './after-telegram-login.component.html',
  styleUrls: ['./after-telegram-login.component.scss']
})
export class AfterTelegramLoginComponent implements OnInit {

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private dashboardService: DashboardService
  ) {
  }

  ngOnInit(): void {
    const obj = this.route.queryParams
      .pipe(map(x => x as TelegramUserLogin))
      .subscribe(value => {
        this.dashboardService.saveSession(value);
        this.router.navigate(['/']).then(r => {
          console.debug('after verify session', r);
          window.location.reload();
        });
      });
  }
}

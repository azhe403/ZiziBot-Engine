import {Component} from '@angular/core';
import {DashboardService} from "../services/dashboard/dashboard.service";
import {Router} from "@angular/router";

@Component({
  selector: 'app-root',
  templateUrl: './root.component.html',
  styleUrls: ['./root.component.scss']
})
export class RootComponent {

  menus: any = [];

  constructor(
    private router: Router,
    private dashboardService: DashboardService
  ) {
    this.buildMenu();
  }

  onLogout() {
    this.dashboardService.logoutSession();
    this.router.navigate(['/']).then(r => {
      console.debug('after-logout', r);
      window.location.reload();
    });
  }

  buildMenu() {
    this.menus = [
      {
        title: 'Mirror User',
        url: '/mirror-user/management',
      },
      {
        title: 'Angular',
        url: '/angular',
      }
    ];
  }
}

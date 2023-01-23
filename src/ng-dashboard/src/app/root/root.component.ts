import {Component} from '@angular/core';
import {DashboardService} from "../services/dashboard/dashboard.service";
import {Router} from "@angular/router";
import {StorageService} from '../services/storage/storage.service';

@Component({
  selector: 'app-root',
  templateUrl: './root.component.html',
  styleUrls: ['./root.component.scss']
})
export class RootComponent {

  sessionId: string | undefined;
  menus: any = [];

  constructor(
    private router: Router,
    private storageService: StorageService,
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
    this.sessionId = this.storageService.get('session_id');

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
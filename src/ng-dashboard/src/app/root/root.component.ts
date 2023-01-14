import {Component} from '@angular/core';
import {DashboardService} from "../services/dashboard/dashboard.service";

@Component({
  selector: 'app-root',
  templateUrl: './root.component.html',
  styleUrls: ['./root.component.scss']
})
export class RootComponent {

  constructor(private dashboardService: DashboardService) {
  }

  onLogout() {
    this.dashboardService.logoutSession();
    alert('logout success!');
  }
}

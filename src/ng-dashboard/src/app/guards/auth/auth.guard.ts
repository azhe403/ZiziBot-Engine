import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot} from '@angular/router';
import {DashboardService} from 'src/app/services/dashboard/dashboard.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private dashboardService: DashboardService) {
  }

  async canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Promise<boolean> {
    const session = await this.dashboardService.checkSession();

    if (!session.isSessionValid)
      return false

    return session.role == 'Sudo';
  }

}
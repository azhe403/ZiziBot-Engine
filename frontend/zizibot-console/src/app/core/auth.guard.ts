import {ActivatedRouteSnapshot, CanActivateChildFn, CanActivateFn, Router} from '@angular/router';
import {DashboardService} from "../features/services/dashboard.service";
import {inject} from "@angular/core";

export const authGuard: CanActivateFn = async (route, state) => {
    return await checkUserRole(route, state);
};

export const authGuardFn: CanActivateChildFn = async (route, state) => {
    return await checkUserRole(route, state);
};

const checkUserRole = async (route: ActivatedRouteSnapshot, url: any) => {
    const dashboardService = inject(DashboardService);
    const router = inject(Router);

    const userSession = await dashboardService.checkBearerSession();

    // @ts-ignore
    if (route.data.role != userSession.roleName) {
        await router.navigate(['/']);
        return false;
    }

    return true;
}

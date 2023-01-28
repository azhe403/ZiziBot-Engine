import {AfterViewInit, Component, OnInit, ViewChild} from '@angular/core';
import {DashboardService} from "../services/dashboard/dashboard.service";
import {NavigationEnd, Router} from "@angular/router";
import {StorageService} from '../services/storage/storage.service';
import {StorageKey} from '../consts/storage-key';

@Component({
  selector: 'app-root',
  templateUrl: './root.component.html',
  styleUrls: ['./root.component.scss']
})
export class RootComponent implements OnInit, AfterViewInit {

  sessionId: string | undefined;
  menus: any = [];

  @ViewChild('drawer') drawer: any;

  constructor(
    private router: Router,
    private storageService: StorageService,
    private dashboardService: DashboardService
  ) {
    this.buildMenu();
  }

  ngAfterViewInit(): void {
    this.loadDrawerState();
  }

  ngOnInit(): void {
    this.router.events.subscribe((val) => {
      if (val instanceof NavigationEnd) {
        this.buildMenu();
      }
    })
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
        title: 'Notes',
        url: '/notes',
      },
      {
        title: 'Angular',
        url: '/angular',
      }
    ];
  }

  toggleDrawer() {
    console.debug('toggleDrawer');

    this.storageService.set(StorageKey.DRAWER_STATE, this.drawer.opened ? 'false' : 'true');
    this.drawer.toggle();
  }

  loadDrawerState() {
    const drawer = this.storageService.get(StorageKey.DRAWER_STATE);
    if (drawer == 'true') {
      this.drawer.open();
    } else {
      this.drawer.close();
    }
  }
}
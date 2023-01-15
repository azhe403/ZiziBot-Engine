import {AfterViewInit, Component, OnInit} from '@angular/core';
import {TelegramUserLogin} from "../../types/TelegramUserLogin";
import {environment} from "../../../environments/environment";
import {DashboardService} from "../../services/dashboard/dashboard.service";
import {ActivatedRoute} from "@angular/router";

@Component({
  selector: 'app-telegram-login-widget',
  templateUrl: './telegram-login-widget.component.html',
  styleUrls: ['./telegram-login-widget.component.scss']
})
export class TelegramLoginWidgetComponent implements OnInit, AfterViewInit {
  public botName = environment.botName;
  public showLoginWidget = false;

  constructor(
    private route: ActivatedRoute,
    private dashboardService: DashboardService
  ) {
  }

  async ngOnInit(): Promise<void> {

  }

  onTelegramAuth(user: TelegramUserLogin) {
    alert('Logged in as ' + user.first_name + ' ' + user.last_name + ' (' + user.id + (user.username ? ', @' + user.username : '') + ')');
  }

  async ngAfterViewInit(): Promise<void> {
    await this.loadWidgetLogin();
  }

  async checkCloudSession() {
    const session = await this.dashboardService.checkSession();
    console.debug('cloud session', session);

    this.showLoginWidget = !session;
  }

  async loadWidgetLogin() {
    await this.checkCloudSession();

    if (!this.showLoginWidget) return;

    setTimeout(() => {
      console.debug('creating widget login', this.botName);
      const loginWidget = document.getElementById('login-widget');
      // @ts-ignore
      loginWidget.setAttribute('data-telegram-login', this.botName);
    }, 0);
  }

}

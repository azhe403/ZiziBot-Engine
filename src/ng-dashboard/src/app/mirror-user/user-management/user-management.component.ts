import {AfterViewInit, Component, OnDestroy} from '@angular/core';
import {MirrorUserService} from "../../services/mirror-user/mirror-user.service";
import {MirrorUser} from "../../types/mirror-user";
import {Subscription} from "rxjs";

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.scss']
})
export class UserManagementComponent implements AfterViewInit, OnDestroy {

  mirrorUser: MirrorUser[] = [];
  mirrorSubscription: Subscription = new Subscription;

  constructor(
    private mirrorUserService: MirrorUserService
  ) {
  }

  ngAfterViewInit(): void {
    this.loadUsers();
  }

  ngOnDestroy(): void {
    this.mirrorSubscription.unsubscribe();
  }

  public loadUsers() {
    this.mirrorSubscription = this.mirrorUserService.getUsers()
      .subscribe(value => {
        this.mirrorUser = value;
      });
  }

}

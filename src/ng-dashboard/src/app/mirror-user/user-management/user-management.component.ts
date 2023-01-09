import {AfterViewInit, Component, OnDestroy} from '@angular/core';
import {MirrorUserService} from "../../services/mirror-user/mirror-user.service";
import {MirrorUser} from "../../types/mirror-user";
import {Subscription} from "rxjs";
import {MatDialog} from "@angular/material/dialog";
import {AddMirrorUserComponent} from "../add-mirror-user/add-mirror-user.component";

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.scss']
})
export class UserManagementComponent implements AfterViewInit, OnDestroy {

  mirrorUser: MirrorUser[] = [];
  mirrorSubscription: Subscription = new Subscription;
  desired_columns: number = 3;

  constructor(
    private matDialog: MatDialog,
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

  deleteUser(userId: number) {
    alert(`delete user ${userId}?`);
    this.mirrorSubscription = this.mirrorUserService.deleteUser(userId)
      .subscribe(value => {
        console.log(value);
        this.loadUsers();
      });
  }

  showAddUserDialog() {
    const dialogRef = this.matDialog.open(AddMirrorUserComponent, {});

    dialogRef.afterClosed().subscribe(result => {
      console.log(`Dialog result: ${result}`);
      if (result) {
        this.loadUsers();
      }
    });
  }
}

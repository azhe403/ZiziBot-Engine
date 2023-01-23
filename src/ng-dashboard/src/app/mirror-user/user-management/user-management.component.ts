import {AfterViewInit, Component, OnDestroy} from '@angular/core';
import {MirrorUserService} from "../../services/mirror-user/mirror-user.service";
import {MirrorUser} from "../../types/mirror-user";
import {Subscription} from "rxjs";
import {MatDialog} from "@angular/material/dialog";
import {AddMirrorUserComponent} from "../add-mirror-user/add-mirror-user.component";
import {ApiResponse} from "../../types/api-response";
import {SwalService} from '../../services/swal/swal.service';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.scss']
})
export class UserManagementComponent implements AfterViewInit, OnDestroy {

  mirrorUser: ApiResponse<MirrorUser[]> | undefined;
  mirrorSubscription: Subscription = new Subscription;
  desired_columns: number = 3;

  constructor(
    private matDialog: MatDialog,
    private swalService: SwalService,
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
    this.swalService.showConfirmation('Delete User', 'Are you sure you want to delete this user?', 'warning')
      .then(result => {
        if (result.isConfirmed) {
          this.mirrorSubscription = this.mirrorUserService.deleteUser(userId)
            .subscribe(value => {
              console.debug('delete mirror user:', value);
              this.loadUsers();
            });
        } else {
          console.debug('User cancelled delete operation');
        }
      });
  }

  showAddUserDialog() {
    const dialogRef = this.matDialog.open(AddMirrorUserComponent, {});

    dialogRef.afterClosed().subscribe(result => {
      console.debug('Dialog result:', result);
      this.loadUsers();
    });
  }
}
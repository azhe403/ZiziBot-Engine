import {Component, Inject, OnInit} from '@angular/core';
import {FormBuilder, FormControl} from "@angular/forms";
import {ApiResponse} from "../../../types/api-response";
import {SwalService} from "../../../services/swal/swal.service";
import {MirrorUserService} from "../../../services/mirror-user/mirror-user.service";
import {AddMirrorUserDto, MirrorUser} from "../../../types/mirror-user";
import {MAT_DIALOG_DATA} from "@angular/material/dialog";
import {of} from "rxjs";

@Component({
  selector: 'app-add-mirror-user',
  templateUrl: './add-mirror-user.component.html',
  styleUrls: ['./add-mirror-user.component.scss']
})
export class AddMirrorUserComponent implements OnInit {
  mirrorUser: ApiResponse<MirrorUser> | undefined;
  entries = Object.entries(this.data);

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private formBuilder: FormBuilder,
    private swalService: SwalService,
    private mirrorUserService: MirrorUserService
  ) {
  }

  ngOnInit(): void {
    console.debug('edit mirror user', this.data);

    if (this.data) {
      this.formOptions.patchValue({
        userId: this.data.userId,
        additionalNote: this.data.additionalNote
      });
    }
  }

  // mirrorUser: AddMirrorUserDto;
  formOptions = this.formBuilder.group({
    userId: new FormControl(),
    monthDuration: new FormControl(),
    additionalNote: new FormControl()
  });

  saveUser() {
    const mirrorUser: AddMirrorUserDto =
      {
        userId: this.formOptions.value.userId,
        monthDuration: this.formOptions.value.monthDuration,
        additionalNote: this.formOptions.value.additionalNote
      };

    this.mirrorUserService.saveUser(mirrorUser)
      .subscribe(value => {
        console.log(value);
        this.swalService.showNotification('Success', 'User added successfully', 'success');
      });
  }

  checkExistingUser($event: any) {
    console.debug('input user id:', $event);

    this.mirrorUserService.getUser($event)
      .subscribe(value => {
        console.debug('user found:', value);
        if (value) {
          this.mirrorUser = value;
          this.swalService.showNotification('User Status', 'User is already added!', 'info');
        }
      });
  }

  protected readonly of = of;
}
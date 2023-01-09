import {Component} from '@angular/core';
import {MirrorUserService} from "../../services/mirror-user/mirror-user.service";
import {AddMirrorUserDto} from "../../types/mirror-user";
import {FormBuilder, FormControl} from "@angular/forms";

@Component({
  selector: 'app-add-mirror-user',
  templateUrl: './add-mirror-user.component.html',
  styleUrls: ['./add-mirror-user.component.scss']
})
export class AddMirrorUserComponent {
  constructor(
    private formBuilder: FormBuilder,
    private mirrorUserService: MirrorUserService
  ) {
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
      });
  }
}

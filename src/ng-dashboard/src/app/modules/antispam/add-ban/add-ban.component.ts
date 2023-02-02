import {Component} from '@angular/core';
import {FormBuilder, FormControl, Validators} from '@angular/forms';
import {MatDialogRef} from '@angular/material/dialog';
import {AntispamService} from '../../../services/antispam/antispam.service';

@Component({
  selector: 'app-add-ban',
  templateUrl: './add-ban.component.html',
  styleUrls: ['./add-ban.component.scss']
})
export class AddBanComponent {

  formOptions = this.formBuilder.group({
    userId: new FormControl('', [Validators.required, Validators.min(1)]),
    content: new FormControl('')
  });

  constructor(
    private dialogRef: MatDialogRef<AddBanComponent>,
    private formBuilder: FormBuilder,
    private antispamService: AntispamService
  ) {
  }

  saveBan() {
    this.antispamService.saveBan({
      userId: this.formOptions.value.userId,
      additionalNote: this.formOptions.value.content
    }).subscribe(x => {
      console.debug('save ban', x);

      this.dialogRef.close({
        success: true
      })
    });
  }
}
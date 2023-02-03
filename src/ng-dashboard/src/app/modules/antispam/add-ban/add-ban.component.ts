import {Component, Inject, OnInit} from '@angular/core';
import {FormBuilder, FormControl, Validators} from '@angular/forms';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {AntispamService} from '../../../services/antispam/antispam.service';

@Component({
  selector: 'app-add-ban',
  templateUrl: './add-ban.component.html',
  styleUrls: ['./add-ban.component.scss']
})
export class AddBanComponent implements OnInit {

  formOptions = this.formBuilder.group({
    userId: new FormControl(123123123, [Validators.required, Validators.min(1)]),
    reason: new FormControl('')
  });

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<AddBanComponent>,
    private formBuilder: FormBuilder,
    private antispamService: AntispamService
  ) {
  }

  ngOnInit(): void {
    console.debug('dialog data', this.data);

    if (this.data) {
      this.formOptions.patchValue({
        userId: this.data.userId,
        reason: this.data.reason
      })
    }
  }

  saveBan() {
    this.antispamService.saveBan({
      userId: this.formOptions.value.userId,
      reason: this.formOptions.value.reason
    }).subscribe(x => {
      console.debug('save ban', x);

      this.dialogRef.close({
        success: true
      })
    });
  }

  onDelete() {
    this.antispamService.deleteFedBan(this.formOptions.value.userId ?? 0)
      .subscribe((response: any) => {
        // this.swalService.showNotification('success', response.message, 'success');
        // this.loadData();
        this.dialogRef.close({
          success: true
        })
      });
  }
}
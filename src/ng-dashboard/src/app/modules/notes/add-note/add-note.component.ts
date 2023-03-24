import {Component} from '@angular/core';
import {FormBuilder, FormControl} from '@angular/forms';
import {NotesService} from '../../../services/notes/notes.service';
import {MatDialogRef} from '@angular/material/dialog';

@Component({
  selector: 'app-add-note',
  templateUrl: './add-note.component.html',
  styleUrls: ['./add-note.component.scss']
})
export class AddNoteComponent {


  constructor(
    private dialogRef: MatDialogRef<AddNoteComponent>,
    private formBuilder: FormBuilder,
    private notesService: NotesService
  ) {
  }

  formOptions = this.formBuilder.group({
    slug: new FormControl(),
    content: new FormControl(),
    fileId: new FormControl(),
    buttonMarkup: new FormControl()
  });

  saveNote() {
    this.notesService.saveNote({
      slug: this.formOptions.value.slug,
      content: this.formOptions.value.content,
      fileId: this.formOptions.value.fileId,
      buttonMarkup: this.formOptions.value.buttonMarkup
    }).subscribe(value => {
      console.debug('save note', value);

      if (value.result) {
        this.dialogRef.close();
      }
    });
  }

}
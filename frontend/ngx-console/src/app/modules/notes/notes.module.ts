import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {NotesManagementComponent} from './notes-management/notes-management.component';
import {AddNoteComponent} from './add-note/add-note.component';
import {NotesRoutingModule} from './notes-routing.module';
import {MatCompoundModule} from '../partial/mat-compound.module';
import {ComponentModule} from "../../components/component.module";


@NgModule({
  declarations: [
    NotesManagementComponent,
    AddNoteComponent
  ],
  imports: [
    CommonModule,
    NotesRoutingModule,
    MatCompoundModule,
    ComponentModule
  ]
})
export class NotesModule {
}
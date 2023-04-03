import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';

import {NotesManagementRoutingModule} from './notes-management-routing.module';
import {NotesManagementComponent} from './notes-management/notes-management.component';


@NgModule({
    declarations: [
        NotesManagementComponent
    ],
    imports: [
        CommonModule,
        NotesManagementRoutingModule
    ]
})
export class NotesManagementModule {
}

import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {DetailNoteComponent} from "./detail-note/detail-note.component";
import {NotesManagementComponent} from "./notes-management/notes-management.component";

const routes: Routes = [
    {path: '', component: NotesManagementComponent},
    {path: ':noteId', component: DetailNoteComponent}
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class NotesManagementRoutingModule {
}

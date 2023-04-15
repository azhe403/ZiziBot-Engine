import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {NotesManagementComponent} from './notes-management/notes-management.component';

const routes: Routes = [
  {path: 'notes-management', component: NotesManagementComponent}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class NotesRoutingModule {
}
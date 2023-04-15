import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';

const routes: Routes = [
    {
        path: 'notes',
        loadChildren: () => import('./notes-management/notes-management.module').then(m => m.NotesManagementModule)
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ChatManagementRoutingModule {
}

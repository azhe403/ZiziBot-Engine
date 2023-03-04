import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';

import {MirrorUserRoutingModule} from './mirror-user-routing.module';
import {UserManagementComponent} from './user-management/user-management.component';
import {MatButtonModule} from "@angular/material/button";
import {MatToolbarModule} from "@angular/material/toolbar";
import {MatIconModule} from "@angular/material/icon";
import {MatGridListModule} from "@angular/material/grid-list";
import {AddMirrorUserComponent} from './add-mirror-user/add-mirror-user.component';
import {MatDialogModule} from "@angular/material/dialog";
import {MatInputModule} from "@angular/material/input";
import {MatSelectModule} from "@angular/material/select";
import {ReactiveFormsModule} from "@angular/forms";
import {AgGridModule} from "ag-grid-angular";


@NgModule({
    declarations: [
        UserManagementComponent,
        AddMirrorUserComponent
    ],
    imports: [
        CommonModule,
        MirrorUserRoutingModule,
        MatButtonModule,
        MatToolbarModule,
        MatIconModule,
        MatGridListModule,
        MatDialogModule,
        MatInputModule,
        MatSelectModule,
        ReactiveFormsModule,
        AgGridModule
    ]
})
export class MirrorUserModule {
}
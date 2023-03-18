import {AfterViewInit, Component, OnDestroy, ViewChild} from '@angular/core';
import {map, Observable, Subscription} from "rxjs";
import {MatDialog} from "@angular/material/dialog";
import {AddMirrorUserComponent} from "../add-mirror-user/add-mirror-user.component";
import {ApiResponse} from "../../../types/api-response";
import {SwalService} from "../../../services/swal/swal.service";
import {MirrorUserService} from "../../../services/mirror-user/mirror-user.service";
import {MirrorUser} from "../../../types/mirror-user";
import {CellClickedEvent, ColDef, GridReadyEvent} from "ag-grid-community";
import {AgGridAngular} from "ag-grid-angular";

@Component({
    selector: 'app-user-management',
    templateUrl: './user-management.component.html',
    styleUrls: ['./user-management.component.scss']
})
export class UserManagementComponent implements AfterViewInit, OnDestroy {

    mirrorUser: ApiResponse<MirrorUser[]> | undefined;
    mirrorSubscription: Subscription = new Subscription;
    desired_columns: number = 3;

    constructor(
        private matDialog: MatDialog,
        private swalService: SwalService,
        private mirrorUserService: MirrorUserService
    ) {
    }

    ngAfterViewInit(): void {
        this.loadUsers();
    }

    ngOnDestroy(): void {
        this.mirrorSubscription.unsubscribe();
    }

    public loadUsers() {
        this.mirrorSubscription = this.mirrorUserService.getUsers()
            .subscribe(value => {
                this.mirrorUser = value;
            });
    }

    deleteUser(userId: number) {
        this.swalService.showConfirmation('Delete User', 'Are you sure you want to delete this user?', 'warning')
            .then(result => {
                if (result.isConfirmed) {
                    this.mirrorSubscription = this.mirrorUserService.deleteUser(userId)
                        .subscribe(value => {
                            console.debug('delete mirror user:', value);
                            this.loadUsers();
                        });
                } else {
                    console.debug('User cancelled delete operation');
                }
            });
    }

    showAddUserDialog() {
        const dialogRef = this.matDialog.open(AddMirrorUserComponent, {
            minWidth: '600px',
            maxWidth: '90%',
            minHeight: '700px',
            maxHeight: '90%',
        });

        dialogRef.afterClosed().subscribe(result => {
            console.debug('Dialog result:', result);
            this.loadUsers();
        });
    }

    // Each Column Definition results in one Column.
    public columnDefs: ColDef[] = [
        {field: 'userId'},
        {field: 'expireDate'},
        {field: 'status'},
        {field: 'memberSince'},
        {field: 'lastUpdate'}
    ];

    // DefaultColDef sets props common to all Columns
    public defaultColDef: ColDef = {
        sortable: true,
        filter: true
    };

    // Data that gets displayed in the grid
    public rowData$!: Observable<any[]>;

    // For accessing the Grid's API
    @ViewChild(AgGridAngular) agGrid!: AgGridAngular;


    // Example load data from sever
    onGridReady(params: GridReadyEvent) {
        this.loadData();
    }

    // Example of consuming Grid Event
    onCellClicked(e: CellClickedEvent): void {
        console.log('cellClicked', e);
        const dialogRef = this.matDialog.open(AddMirrorUserComponent, {
            minWidth: '600px',
            maxWidth: '90%',
            minHeight: '700px',
            maxHeight: '90%',
            data: e.data
        });

        dialogRef.afterClosed()
            .subscribe(result => {
                console.log(`edit result: `, result);
                this.loadData();
            });
    }

    // Example using Grid's API
    clearSelection(): void {
        this.agGrid.api.deselectAll();
    }

    ngOnInit(): void {

    }

    loadData() {
        this.rowData$ = this.mirrorUserService.getUsers()
            .pipe(map((response: ApiResponse<MirrorUser[]>) => {
                return response.result;
            }));
    }

    // showAddUserDialog() {
    //     const dialogRef: MatDialogRef<AddBanComponent, boolean> = this.matDialog.open(AddBanComponent, {
    //         width: '90%',
    //         maxWidth: '600px',
    //         minHeight: '300px',
    //         height: '50%'
    //     });
    //
    //     dialogRef.afterClosed()
    //         .subscribe(result => {
    //             console.log(`insert result: `, result);
    //             this.loadData();
    //         });
    // }

    // showDeleteDialog() {
    //     const selectedRows = this.agGrid.api.getSelectedRows();
    //
    //     if (selectedRows.length <= 0) {
    //         console.log('No rows selected');
    //         return;
    //     }
    //
    //     console.debug('selectedRows', selectedRows);
    //
    //     const first = selectedRows[0];
    //
    //     this.swalService.showConfirmation('delete', `Are you sure you want to delete this userId: ${first.userId}?`, "question")
    //         .then((result) => {
    //             if (result.isConfirmed) {
    //                 this.antispamService.deleteFedBan(first.userId)
    //                     .subscribe((response: any) => {
    //                         this.swalService.showNotification('success', response.message, 'success');
    //                         this.loadData();
    //                     });
    //             }
    //         });
    // }

    onRefresh() {
        this.loadData();
    }
}
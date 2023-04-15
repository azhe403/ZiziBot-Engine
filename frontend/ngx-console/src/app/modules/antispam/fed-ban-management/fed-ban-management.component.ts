import {Component, OnInit, ViewChild} from '@angular/core';
import {AgGridAngular} from 'ag-grid-angular';
import {CellClickedEvent, ColDef, GridReadyEvent} from 'ag-grid-community';
import {map, Observable} from 'rxjs';
import {HttpClient} from '@angular/common/http';
import {AntispamService} from '../../../services/antispam/antispam.service';
import {ApiResponse} from '../../../types/api-response';
import {Antispam} from '../../../types/antispam';
import {MatDialog, MatDialogRef} from '@angular/material/dialog';
import {AddBanComponent} from '../add-ban/add-ban.component';
import {SwalService} from '../../../services/swal/swal.service';

@Component({
  selector: 'app-fed-ban-management',
  templateUrl: './fed-ban-management.component.html',
  styleUrls: ['./fed-ban-management.component.scss']
})
export class FedBanManagementComponent implements OnInit {

  constructor(
    private http: HttpClient,
    private matDialog: MatDialog,
    private antispamService: AntispamService,
    private swalService: SwalService
  ) {
  }

  // Each Column Definition results in one Column.
  public columnDefs: ColDef[] = [
    {field: 'userId'},
    {field: 'chatId'},
    {field: 'reason'},
    {field: 'status'},
    {field: 'createdDate'},
    {field: 'updatedDate'}
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
    const dialogRef = this.matDialog.open(AddBanComponent, {
      width: '90%',
      maxWidth: '600px',
      minHeight: '300px',
      height: '50%',
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
    this.rowData$ = this.antispamService.getFedBanList()
      .pipe(map((response: ApiResponse<Antispam[]>) => {
        return response.result;
      }));
  }

  showAddUserDialog() {
    const dialogRef: MatDialogRef<AddBanComponent, boolean> = this.matDialog.open(AddBanComponent, {
      width: '90%',
      maxWidth: '600px',
      minHeight: '300px',
      height: '50%'
    });

    dialogRef.afterClosed()
      .subscribe(result => {
        console.log(`insert result: `, result);
        this.loadData();
      });
  }

  showDeleteDialog() {
    const selectedRows = this.agGrid.api.getSelectedRows();

    if (selectedRows.length <= 0) {
      console.log('No rows selected');
      return;
    }

    console.debug('selectedRows', selectedRows);

    const first = selectedRows[0];

    this.swalService.showConfirmation('delete', `Are you sure you want to delete this userId: ${first.userId}?`, "question")
      .then((result) => {
        if (result.isConfirmed) {
          this.antispamService.deleteFedBan(first.userId)
            .subscribe((response: any) => {
              this.swalService.showNotification('success', response.message, 'success');
              this.loadData();
            });
        }
      });
  }

  onRefresh() {
    this.loadData();
  }

  restoreDelete() {
    const selectedRows = this.agGrid.api.getSelectedRows();

    if (selectedRows.length <= 0) {
      console.log('No rows selected');
      return;
    }

    // console.debug('selectedRows', selectedRows);
    //
    // const first = selectedRows[0];
    //
    // this.swalService.showConfirmation('restore', `Are you sure you want to restore this userId: ${first.userId}?`, "question")
    //   .then((result) => {
    //     if (result.isConfirmed) {
    //       this.antispamService.restoreFedBan(first.userId)
    //         .subscribe((response: any) => {
    //           this.swalService.showNotification('success', response.message, 'success');
    //           this.loadData();
    //         });
    //     }
    //   });
  }
}
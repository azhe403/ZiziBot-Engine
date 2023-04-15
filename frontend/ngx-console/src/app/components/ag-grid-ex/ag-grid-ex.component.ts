import {Component, Input, Output} from '@angular/core';

@Component({
  selector: 'app-ag-grid-ex',
  templateUrl: './ag-grid-ex.component.html',
  styleUrls: ['./ag-grid-ex.component.scss']
})
export class AgGridExComponent {

  @Input() defaultColDef: any;
  @Input() columnDefs: any;
  @Input() rowData: any;

  @Output() onGridReady: any;
  @Output() onSelectionChanged: any;
  @Output() onCellClicked: any;

}
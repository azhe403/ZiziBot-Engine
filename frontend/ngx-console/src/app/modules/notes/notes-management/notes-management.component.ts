import {AfterViewInit, Component, OnInit} from "@angular/core";
import {MatDialog} from "@angular/material/dialog";
import {AddNoteComponent} from "../add-note/add-note.component";
import {TelegramService} from "../../../services/telegram/telegram.service";

@Component({
  selector: "app-notes-management",
  templateUrl: "./notes-management.component.html",
  styleUrls: ["./notes-management.component.scss"],
})
export class NotesManagementComponent implements OnInit, AfterViewInit {
  constructor(
    private matDialog: MatDialog,
    private telegramService: TelegramService
  ) {
  }

  ngAfterViewInit(): void {
  }

  ngOnInit(): void {
  }

  showAddNoteDialog() {
    const dialogRef = this.matDialog.open(AddNoteComponent, {
      width: "90%",
      maxWidth: "600px",
    });

    dialogRef.afterClosed().subscribe((result) => {
      console.log(`Dialog result: ${result}`);
    });
  }
}
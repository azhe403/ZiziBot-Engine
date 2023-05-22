import {AfterViewInit, Component} from '@angular/core';
import {Router} from "@angular/router";
import {MessageService} from "primeng/api";
import {Note} from "projects/zizibot-types/src/restapi/note";
import {ChatService} from "../../../../services/chat.service";

@Component({
    selector: 'app-notes-management',
    templateUrl: './notes-management.component.html',
    styleUrls: ['./notes-management.component.scss'],
    providers: [MessageService]
})
export class NotesManagementComponent implements AfterViewInit {
    selectedChatId = 0;
    loading: any;
    listNote: Note[] = [];
    selectedNote: Note = {} as Note;

    constructor(private router: Router, private chatService: ChatService) {
    }

    ngAfterViewInit(): void {
        console.debug('ngAfterViewInit');
    }

    loadNote() {
        this.chatService.getNote(this.selectedChatId).subscribe((response) => {
            console.debug('list note', response);

            this.listNote = response.result;
        });
    }

    onOpenEditor(note: any) {
        console.debug('Open Note editor', note);

        this.router.navigate(['/chat/notes/', note.id]).then(r => console.debug('navigate to Note editor', r));
    }

    onSelectedChatId($event: number) {
        this.chatService.getNote($event).subscribe((res) => {
            console.log('list Note', res);
            this.listNote = res.result;
        });
    }
}

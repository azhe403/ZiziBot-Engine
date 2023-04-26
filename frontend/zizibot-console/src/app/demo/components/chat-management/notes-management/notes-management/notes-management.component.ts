import {AfterViewInit, Component} from '@angular/core';
import {Router} from "@angular/router";
import {MessageService} from "primeng/api";
import {Note} from "projects/zizibot-types/src/restapi/note";
import {ChatService} from "../../../../service/chat.service";

@Component({
    selector: 'app-notes-management',
    templateUrl: './notes-management.component.html',
    styleUrls: ['./notes-management.component.scss'],
    providers: [MessageService]
})
export class NotesManagementComponent implements AfterViewInit {
    selectedChatId = 0;
    loading: any;
    listWelcomeMessage: Note[] = [];

    constructor(private router: Router, private chatService: ChatService) {
    }


    ngAfterViewInit(): void {

    }

    loadNote() {
        this.chatService.getNote(this.selectedChatId).subscribe((response) => {
            console.debug('list note', response);

            this.listWelcomeMessage = response.result;
        });
    }

    onSelectChange($event: number) {
        console.debug('onSelectChange', $event);
        this.selectedChatId = $event;

        this.loadNote();
    }

    onOpenEditor(note: any) {
        console.debug('Open Note editor', note);

        this.router.navigate(['/chat/notes/', note.id]).then(r => console.debug('navigate to Note editor', r));
    }

}

import {AfterViewInit, Component} from '@angular/core';
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

    constructor(private chatService: ChatService) {
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

    onOpenEditor(welcome: any) {

    }

    onSelectWelcome(welcome: any) {

    }

}

import {Component} from '@angular/core';
import {MessageService} from "primeng/api";
import {ChatService} from "../../../../services/chat.service";
import {Rss} from "../../../../../../../projects/zizibot-types/src/restapi/rss";

@Component({
    selector: 'app-list-rss',
    templateUrl: './list-rss.component.html',
    styleUrls: ['./list-rss.component.scss'],
    providers: [
        MessageService
    ]
})
export class ListRssComponent {
    listRss: Rss[] = [];
    selectedRss: Rss = {} as Rss;

    constructor(private chatService: ChatService) {
    }

    onSelectedChatId($event: number) {
        this.chatService.getListRss($event).subscribe((res) => {
            console.log('list RSS', res);
            this.listRss = res.result;
        });
    }
}

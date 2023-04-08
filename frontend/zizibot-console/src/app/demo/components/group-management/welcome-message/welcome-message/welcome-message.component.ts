import {AfterViewInit, Component} from '@angular/core';
import {GroupService} from "../../../../service/group.service";
import {MessageService} from "primeng/api";
import {WelcomeMessage} from "zizibot-contracts/src/restapi/welcome-message";
import {Router} from "@angular/router";

@Component({
    selector: 'app-welcome-message',
    templateUrl: './welcome-message.component.html',
    styleUrls: ['./welcome-message.component.scss'],
    providers: [MessageService]
})
export class WelcomeMessageComponent implements AfterViewInit {
    loading: boolean = true;
    listWelcomeMessage: WelcomeMessage[] = [];
    selectedWelcome: WelcomeMessage | undefined;
    chatId: number = 0;

    constructor(private router: Router, private messageService: MessageService, private groupService: GroupService) {
    }

    ngAfterViewInit(): void {
        // this.loadWelcomeMessage();
    }

    loadWelcomeMessage() {
        this.groupService.getWelcomeMessage(this.chatId).subscribe((response) => {
            console.debug('welcome message', response);

            this.loading = false;
            this.listWelcomeMessage = response.result;
        });
    }

    onSelectWelcome(welcome: any) {
        console.debug('onSelectWelcome', welcome);
        this.groupService.selectWelcomeMessage({
            chatId: this.chatId,
            welcomeId: welcome.id,
        }).subscribe((response) => {
            this.messageService.add({severity: 'success', summary: 'Success', detail: response.message});

            this.loadWelcomeMessage();
        });
    }

    onOpenEditor(welcome: any) {
        console.debug('onOpenEditor', welcome);
        this.router.navigate(['/group/welcome-message', welcome.id]).then(r => console.debug('after navigate', r));
    }

    onSelectChange($event: number) {
        console.debug('onSelectChange', $event);
        this.chatId = $event;
        this.loadWelcomeMessage();
    }
}

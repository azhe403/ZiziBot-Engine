import {AfterViewInit, Component} from '@angular/core';
import {GroupService} from "../../../../service/group.service";
import {MessageService} from "primeng/api";
import {WelcomeMessage} from "zizibot-contracts/src/restapi/welcome-message";

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

    constructor(private messageService: MessageService, private groupService: GroupService) {
    }

    ngAfterViewInit(): void {
        this.loadWelcomeMessage();
    }

    private loadWelcomeMessage() {
        this.groupService.getWelcomeMessage().subscribe((response) => {
            console.debug('welcome message', response);

            this.loading = false;
            this.listWelcomeMessage = response.result;
        });
    }

    onSelectWelcome(welcome: any) {
        console.debug('onSelectWelcome', welcome);
        this.groupService.selectWelcomeMessage(welcome.id).subscribe((response) => {
            this.messageService.add({severity: 'success', summary: 'Success', detail: response.message});

            this.loadWelcomeMessage();
        });
    }

    onOpenEditor(welcome: any) {
        console.debug('onOpenEditor', welcome);
    }
}

import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {GroupService} from 'src/app/demo/service/group.service';
import {MessageService} from "primeng/api";

@Component({
    selector: 'app-detail-welcome-message',
    templateUrl: './detail-welcome-message.component.html',
    styleUrls: ['./detail-welcome-message.component.scss'],
    providers: [MessageService]
})
export class DetailWelcomeMessageComponent implements OnInit {

    welcomeId: string | null | undefined;
    pageTitle = 'Add Welcome Message';
    formGroup: FormGroup = new FormGroup({});
    mediaTypes: any;

    constructor(private route: ActivatedRoute, private router: Router, private messageService: MessageService, private groupService: GroupService) {
    }

    ngOnInit(): void {
        this.getParam();
        this.initForm();
        this.initMediaTypes();
    }

    private loadWelcomeMessage() {
        this.groupService.getWelcomeMessageById(this.welcomeId)
            .subscribe({
                next: (response) => {
                    console.debug('welcome message', response);

                    if (response.result)
                        this.formGroup.patchValue(response.result);
                },
                error: (err) => {
                    console.error('detail welcome message', err);
                    this.messageService.add({severity: 'error', summary: err.statusText, detail: err.error.message});
                },
                complete: () => console.info('get welcome message complete')
            });
    }

    private initMediaTypes() {
        this.mediaTypes = [
            {name: 'Text', value: 1},
            {name: 'Image', value: 2},
            {name: 'Audio', value: 3},
            {name: 'Video', value: 4},
            {name: 'Document', value: 6},
            {name: 'Sticker', value: 7},
        ];
    }

    private getParam() {
        this.route.paramMap.subscribe(params => {
            console.debug('params', params);

            this.welcomeId = params.get('welcomeId');
            console.debug('welcomeId:', this.welcomeId);

            if (this.welcomeId) {
                this.pageTitle = 'Edit Welcome Message';
                this.loadWelcomeMessage();
            }

        });
    }

    private initForm() {
        this.formGroup.addControl('welcomeId', new FormControl('', [Validators.required]))
        this.formGroup.addControl('chatId', new FormControl(0, [Validators.required]));
        this.formGroup.addControl('text', new FormControl('', [Validators.required]));
        this.formGroup.addControl('media', new FormControl(''));
        this.formGroup.addControl('dataType', new FormControl(1));
        this.formGroup.addControl('rawButton', new FormControl(''));
    }

    onSubmit() {
        console.debug('formGroup', this.formGroup);

        this.groupService.saveWelcomeMessage({
            id: this.welcomeId,
            chatId: this.formGroup.value.chatId,
            text: this.formGroup.value.text,
            rawButton: this.formGroup.value.rawButton,
            media: this.formGroup.value.media,
            dataType: this.formGroup.value.dataType,
        }).subscribe({
            next: (response) => {
                console.debug('welcome message', response);

                this.router.navigate(['/group/welcome-message']).then(r => console.debug('after submit', r));
            },
            error: (err) => {
                console.error('detail welcome message', err);
                this.messageService.add({severity: 'error', summary: err.statusText, detail: err.error.result.error});
            },
            complete: () => {
                console.info('get welcome message complete');
            }
        });

    }
}

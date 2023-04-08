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
            {name: 'Text', value: 'text'},
            {name: 'Image', value: 'image'},
            {name: 'Video', value: 'video'},
            {name: 'Audio', value: 'audio'},
            {name: 'Document', value: 'document'},
            {name: 'Sticker', value: 'sticker'},
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
        this.formGroup.addControl('chatId', new FormControl('', [Validators.required]));
        this.formGroup.addControl('text', new FormControl('', [Validators.required]));
        this.formGroup.addControl('media', new FormControl(''));
        this.formGroup.addControl('dataType', new FormControl(''));
        this.formGroup.addControl('rawButton', new FormControl(''));
    }

    onSubmit() {
        console.debug('formGroup', this.formGroup);

        this.router.navigate(['/group/welcome-message']).then(r => console.debug('after submit', r));
    }
}

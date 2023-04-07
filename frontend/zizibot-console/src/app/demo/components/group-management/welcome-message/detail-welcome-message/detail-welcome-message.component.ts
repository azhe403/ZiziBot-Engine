import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {FormControl, FormGroup, Validators} from "@angular/forms";

@Component({
    selector: 'app-detail-welcome-message',
    templateUrl: './detail-welcome-message.component.html',
    styleUrls: ['./detail-welcome-message.component.scss']
})
export class DetailWelcomeMessageComponent implements OnInit {

    welcomeId: string | null | undefined;
    pageTitle = 'Add Welcome Message';
    formGroup: FormGroup = new FormGroup({});
    mediaTypes: any;

    constructor(private route: ActivatedRoute, private router: Router) {
    }

    ngOnInit(): void {
        this.getParam();
        this.initForm();
        this.initMediaTypes();
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
            }
        });
    }

    private initForm() {
        this.formGroup.addControl('welcomeId', new FormControl('', [Validators.required]))
        this.formGroup.addControl('chatId', new FormControl('', [Validators.required]));
        this.formGroup.addControl('text', new FormControl('', [Validators.required]));
        this.formGroup.addControl('mediaId', new FormControl(''));
        this.formGroup.addControl('mediaType', new FormControl(''));
        this.formGroup.addControl('rawButton', new FormControl(''));
    }

    onSubmit() {
        console.debug('formGroup', this.formGroup);

        this.router.navigate(['/group/welcome-message']).then(r => console.debug('after submit', r));
    }
}

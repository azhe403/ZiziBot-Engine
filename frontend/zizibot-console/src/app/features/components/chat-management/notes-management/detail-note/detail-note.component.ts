import {AfterViewInit, Component, OnInit} from '@angular/core';
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {ActivatedRoute, Router} from "@angular/router";
import Enumerable from "linq";
import {ConfirmationService, MessageService} from "primeng/api";
import {MediaType} from "projects/zizibot-types/src/restapi/media-type";
import {ChatService} from "../../../../../demo/service/chat.service";

@Component({
    selector: 'app-detail-note',
    templateUrl: './detail-note.component.html',
    styleUrls: ['./detail-note.component.scss'],
    providers: [MessageService, ConfirmationService]
})
export class DetailNoteComponent implements AfterViewInit, OnInit {
    noteId: string | null | undefined;
    pageTitle: string | undefined;
    formGroup: FormGroup = new FormGroup({});
    mediaTypes: MediaType[] = [];

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private messageService: MessageService,
        private confirmationService: ConfirmationService,
        private chatService: ChatService
    ) {
    }

    ngOnInit(): void {
        this.initMediaTypes();
        this.initForm();
        this.getParam();
    }

    ngAfterViewInit(): void {
    }

    private getParam() {
        this.route.paramMap.subscribe(params => {
            console.debug('params', params);

            this.noteId = params.get('noteId');
            console.debug('noteId:', this.noteId);

            if (this.noteId) {
                this.pageTitle = 'Edit Note';
                this.loadData();
            }

        });
    }

    private initMediaTypes() {
        this.mediaTypes = [
            {name: 'Teks', value: 1},
            {name: 'Foto', value: 2},
            {name: 'Audio', value: 3},
            {name: 'Video', value: 4},
            {name: 'Dokumen', value: 6},
            {name: 'Stiker', value: 7},
        ];
    }

    private initForm() {
        this.formGroup.addControl('noteId', new FormControl('', [Validators.required]))
        this.formGroup.addControl('chatId', new FormControl(0, [Validators.required]));
        this.formGroup.addControl('chatTitle', new FormControl('', [Validators.required]));
        this.formGroup.addControl('query', new FormControl('', [Validators.required]));
        this.formGroup.addControl('text', new FormControl('', [Validators.required]));
        this.formGroup.addControl('media', new FormControl(''));
        this.formGroup.addControl('dataType', new FormControl<MediaType | null>(Enumerable.from(this.mediaTypes).first()));
        this.formGroup.addControl('rawButton', new FormControl(''));

        this.formGroup.controls['chatTitle'].disable();
    }

    private loadData() {
        this.chatService.getNoteById(this.noteId)
            .subscribe({
                next: (response) => {
                    console.debug('note', response.result);

                    if (response.result) {
                        console.debug('patch form for edit');
                        const welcome = response.result;
                        this.formGroup.patchValue(welcome);
                        this.formGroup.patchValue({
                            dataType: Enumerable.from(this.mediaTypes).firstOrDefault(x => x.value == welcome.dataType),
                        });
                    }
                },
                error: (err) => {
                    console.error('detail note', err);
                    this.messageService.add({severity: 'error', summary: err.statusText, detail: err.error.message});
                },
                complete: () => console.info('get note complete')
            });
    }

    onSubmit() {
        console.debug('formGroup', this.formGroup);

        this.chatService.saveNote({
            id: this.noteId,
            chatId: this.formGroup.value.chatId,
            query: this.formGroup.value.query,
            content: this.formGroup.value.text,
            rawButton: this.formGroup.value.rawButton,
            media: this.formGroup.value.media,
            dataType: this.formGroup.value.dataType.value,
        }).subscribe({
            next: (response) => {
                console.debug('note', response);

                this.router.navigate(['/chat/notes']).then(r => console.debug('after submit', r));
            },
            error: (err) => {
                console.error('detail note', err);
                this.messageService.add({severity: 'error', summary: err.statusText, detail: err.error.result.error});
            },
            complete: () => {
                console.info('get note complete');
            }
        });

    }

    onDelete() {
        console.debug('delete welcome message', this.noteId);

        this.confirmationService.confirm({
            message: 'Apakah kamu yakin akan menghapus Note ini?',
            header: 'Konfirmasi Hapus',
            icon: 'pi pi-info-circle',
            accept: () => {
                console.debug('accept to delete note', this.noteId);
                this.chatService.deleteNote({
                    id: this.noteId,
                    chatId: this.formGroup.value.chatId,
                }).subscribe({
                    next: (response) => {
                        console.debug('note', response);
                    },
                    error: (err) => {
                        console.error('delete note', err);
                        this.messageService.add({severity: 'error', summary: err.statusText, detail: err.error.result.error});
                    },
                    complete: () => {
                        console.info('delete note complete');
                        this.router.navigate(['/chat/notes']).then(r => console.debug('after submit', r));
                    }
                })
            },
            reject: (type: any) => {
                console.debug('user reject delete note', type);
            }
        });
    }
}

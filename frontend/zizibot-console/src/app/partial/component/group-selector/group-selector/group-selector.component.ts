import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {ControlValueAccessor, NG_VALUE_ACCESSOR} from "@angular/forms";
import {GroupService} from "../../../../demo/service/group.service";
import {TelegramService} from "../../../services/telegram.service";
import {TelegramGroup} from "../../../../../../projects/zizibot-types/src/restapi/telegram-group";
import Enumerable from "linq";

@Component({
    selector: 'app-group-selector',
    templateUrl: './group-selector.component.html',
    styleUrls: ['./group-selector.component.scss'],
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: GroupSelectorComponent,
            multi: true
        }
    ]
})
export class GroupSelectorComponent implements OnInit, ControlValueAccessor {

    mediaTypes: any;
    listGroup: TelegramGroup[] = [];
    chatId = 0;

    @Output() selectedChatId = new EventEmitter<number>();

    constructor(private groupService: GroupService, private telegramService: TelegramService) {
    }

    onChange = (chatId: any) => {
        this.selectedChatId.emit(chatId);
    };

    onTouched = () => {
    };

    touched = false;
    disabled = false;

    ngOnInit(): void {
        this.initMediaTypes();
        this.loadListGroup();
    }

    writeValue(chatId: number) {
        console.debug('writeValue', chatId);
        this.chatId = chatId;
    }

    registerOnChange(onChange: any) {
        console.debug('registerOnChange', onChange);
        this.onChange = onChange;
    }

    registerOnTouched(onTouched: any) {
        console.debug('registerOnTouched', onTouched);
        this.onTouched = onTouched;
    }

    markAsTouched() {
        if (!this.touched) {
            this.onTouched();
            this.touched = true;
        }
    }

    setDisabledState(disabled: boolean) {
        this.disabled = disabled;
    }

    loadListGroup() {
        this.telegramService.listTelegramGroup().subscribe((data) => {
            console.debug('listTelegramGroup', data);
            this.listGroup = data.result;

            if (this.chatId == 0)
                this.onChange(Enumerable.from(this.listGroup).first().chatId);
        });
    }

    private initMediaTypes() {
        this.mediaTypes = [
            {name: 'Text', value: '1'},
            {name: 'Image', value: '2'},
            {name: 'Video', value: '3'},
            {name: 'Audio', value: '4'},
            {name: 'Document', value: '5'},
            {name: 'Sticker', value: '6'},
        ];
    }

    onSelectChange($event: Event) {
        // @ts-ignore
        console.debug('select group:', $event.value);
        // @ts-ignore
        this.chatId = $event.value.chatId;

        this.onChange(this.chatId);
    }
}
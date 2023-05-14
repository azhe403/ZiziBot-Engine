import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ControlValueAccessor, NG_VALUE_ACCESSOR} from "@angular/forms";
import Enumerable from "linq";
import {MessageService} from "primeng/api";
import {ConditionState} from "projects/zizibot-types/src/constant/state";
import {StorageKey} from "projects/zizibot-types/src/constant/storage-key";
import {TelegramGroup} from "projects/zizibot-types/src/restapi/telegram-group";
import {TelegramService} from "../../../services/telegram.service";
import {GroupService} from "../../../../features/services/group.service";

@Component({
    selector: 'app-group-selector',
    templateUrl: './group-selector.component.html',
    styleUrls: ['./group-selector.component.scss'],
    providers: [
        MessageService,
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: GroupSelectorComponent,
            multi: true
        }
    ]
})
export class GroupSelectorComponent implements OnInit, ControlValueAccessor {

    ConditionState = ConditionState;
    selectorState = -1;
    listGroup: TelegramGroup[] = [];
    selectedGroup: TelegramGroup | undefined;
    chatId = 0;
    touched = false;

    @Input() disabled = false;
    @Input() disableSelector: boolean = false;
    @Output() selectedChatId = new EventEmitter<number>();
    @Output() selectedChat = new EventEmitter<TelegramGroup>();

    constructor(
        private groupService: GroupService,
        private telegramService: TelegramService,
        private messageService: MessageService
    ) {
    }

    ngOnInit(): void {
        this.loadListGroup();
    }

    onChange = (chatId: any) => {
        localStorage.setItem(StorageKey.SELECTED_CHAT_ID, chatId.toString());
        this.selectedGroup = Enumerable.from(this.listGroup).firstOrDefault(x => x.chatId == chatId);

        this.selectedChatId.emit(chatId);
    };

    onTouched = () => {
        console.debug('onTouched');
    };

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

            if (!data.result) {
                this.selectorState = 0
                return this.messageService.add({severity: 'warn', summary: 'Warning', detail: 'Sepertinya Anda belum memiliki grub'});
            }

            this.selectorState = 2;
            this.listGroup = data.result;

            const selectedChatId = localStorage.getItem(StorageKey.SELECTED_CHAT_ID);
            console.debug('selectedChatId', selectedChatId);

            if (this.listGroup.length > 0) {
                if (selectedChatId != null) {
                    console.debug('update selected from local storage', selectedChatId);
                    this.onChange(selectedChatId);
                } else {
                    console.debug('update selected from first group');
                    this.onChange(Enumerable.from(this.listGroup).first().chatId);
                }
            }
        });
    }

    onSelectChange($event: Event) {
        console.debug('onSelectChange', $event);

        console.debug('onSelectChange', this.selectedGroup);
        this.selectedChat.emit(this.selectedGroup);
        this.onChange(this.selectedGroup?.chatId);
    }
}

import { Component, EventEmitter, OnInit, Output } from "@angular/core";
import { TelegramGroup } from "../../types/telegram-group";
import { TelegramService } from "../../services/telegram/telegram.service";

@Component({
  selector: "app-group-selector",
  templateUrl: "./group-selector.component.html",
  styleUrls: ["./group-selector.component.scss"],
})
export class GroupSelectorComponent implements OnInit {
  listGroup: TelegramGroup[] | undefined;
  selected: number | undefined;
  showLoading: boolean = true;

  @Output() selectedGroup = new EventEmitter<number>();

  constructor(private telegramService: TelegramService) {}

  ngOnInit(): void {
    this.loadListGroup();
  }

  loadListGroup() {
    this.telegramService.listTelegramGroup().subscribe((data) => {
      this.listGroup = data.result;
      this.showLoading = false;

      if (!this.selected) this.selected = this.listGroup[0].chatId;
    });
  }

  onSelect(selected: number | undefined) {
    console.debug("selected group", selected);
    this.selectedGroup.emit(selected);
  }
}
import {AfterViewInit, Component} from '@angular/core';
import {GroupService} from 'src/app/services/group/group.service';
import {WelcomeMessage} from "../../../types/welcome-message";

@Component({
  selector: 'app-welcome-management',
  templateUrl: './welcome-management.component.html',
  styleUrls: ['./welcome-management.component.scss']
})
export class WelcomeManagementComponent implements AfterViewInit {

  welcomeMessages: WelcomeMessage[] = [];

  public constructor(private groupService: GroupService) {
  }


  ngAfterViewInit(): void {
    this.groupService.getWelcomeMessages(1).subscribe((data) => {
      this.welcomeMessages = data.result;
    });
  }

}
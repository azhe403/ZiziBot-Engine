import {Component, OnInit} from '@angular/core';
import {LogSignalrService} from "../../../../services/log-signalr.service";
import {HttpClient} from "@angular/common/http";
import {Logging} from "../../../../../../../projects/zizibot-types/src/restapi/logging";

@Component({
    selector: 'app-log-viewer',
    templateUrl: './log-viewer.component.html',
    styleUrls: ['./log-viewer.component.scss']
})
export class LogViewerComponent implements OnInit {

    logs: Logging[] = [];
    cols: any[] = [];
    status: string = 'Sedang menunggu data..';

    constructor(private logSignalrService: LogSignalrService, private http: HttpClient) {
    }

    ngOnInit() {
        this.logSignalrService.startConnection();
        this.startHttpRequest();

        this.cols = [
            {field: 'id', header: 'ID'},
            {field: 'level', header: 'Level'},
            {field: 'timestamp', header: 'Timestamp'},
            {field: 'message', header: 'Message'},
        ];
    }

    private startHttpRequest = () => {
        console.debug('Starting Live-view log..')
        this.logSignalrService.hubConnection.start()
            .then(() => {
                console.debug('Connection started');
                this.status = 'Tersambung ke sumber..';
                this.logSignalrService.hubConnection.on('SendLogAsObject', (data: Logging) => {
                    // console.debug('log-object-co', data);
                    this.logs.unshift(data);
                });
            })
            .catch(err => console.log('Error while starting connection: ' + err));
    }
}

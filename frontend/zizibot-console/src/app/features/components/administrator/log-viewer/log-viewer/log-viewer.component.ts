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

    logsAll: Logging[] = [];
    filteredLogs: Logging[] = [];
    cols: any[] = [];
    status: string = 'Sedang menunggu data..';
    searchLog: string | undefined;

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
                    this.logsAll.unshift(data);
                    // this.filteredLogs = this.logsAll.filter(element => dayjs(element.timestamp).isAfter(dayjs().subtract(5,'minute')));
                    this.filteredLogs = this.logsAll;
                    this.onSearchLog();
                });
            })
            .catch(err => console.log('Error while starting connection: ' + err));
    }

    onSearchLog() {
        if (this.searchLog) {
            this.filteredLogs = this.filteredLogs.filter(element => element.message.toLowerCase().includes(<string>this.searchLog?.toLowerCase()));
        }
    }
}

import {Injectable} from '@angular/core';
import {HubConnection, HubConnectionBuilder} from "@microsoft/signalr";

@Injectable({
    providedIn: 'root'
})
export class LogSignalrService {

    // @ts-ignore
    hubConnection: HubConnection;

    constructor() {
        // empty
    }

    public startConnection = () => {
        this.hubConnection = new HubConnectionBuilder()
            .withUrl('/api/logging')
            .withAutomaticReconnect()
            .build();
    }
}

import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { User } from '../_models/user';

export function startHubConnection(user: User, hubUrl: string) {
    const hubConnection = new HubConnectionBuilder()
        .withUrl(hubUrl, { accessTokenFactory: () => user.token })
        .withAutomaticReconnect()
        .build();

    hubConnection.start().catch(error => console.log(error));

    return hubConnection;
}
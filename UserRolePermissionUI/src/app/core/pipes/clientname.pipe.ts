import { Pipe, PipeTransform } from '@angular/core';
import { Client } from '../models/client.model';
import { ClientService } from '../services/client.service';
import { Observable, of, map } from 'rxjs';

@Pipe({ name: 'clientName', standalone: true })
export class ClientNamePipe implements PipeTransform {
  constructor(private clientService: ClientService) {}

  transform(clientId: number, clients: Client[]): Observable<string> {
    const client = clients.find((c) => c.id === clientId);
    if (client) {
      return of(client.name);
    } else {
      return this.clientService.getClientById(clientId).pipe(
        map(response => response.Data ? response.Data.name : 'Unknown')
      );
    }
  }
}

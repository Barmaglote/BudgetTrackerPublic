import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { TransferItem } from 'src/app/shared/models/transfer-item';
import { TableLazyLoadEvent } from 'primeng/table';
import { TransfersResponse } from '../interfaces/items-response.interface';

@Injectable()
export class TransferService {
  private controllerAPIUrl: string;

  constructor(private http: HttpClient) {
    this.controllerAPIUrl = `${environment.webapi}`;
  }

  addItem(item: TransferItem, creditId?: string): Observable<boolean> {
    if (creditId) {
      return this.http.post<boolean>(`${this.controllerAPIUrl}/transfer?creditId=${creditId}`, item);
    } else {
      return this.http.post<boolean>(`${this.controllerAPIUrl}/transfer`, item);
    }

  }

  getItem(id: string): Observable<TransferItem> {
    return this.http.get<TransferItem>(`${this.controllerAPIUrl}/transfer/${id}`);
  }

  getItems(tableLazyLoadEvent: TableLazyLoadEvent): Observable<TransfersResponse> {
    return this.http.post<TransfersResponse>(`${this.controllerAPIUrl}/transfer/items/`, tableLazyLoadEvent);
  }

  deleteItem(id: string): Observable<boolean> {
    return this.http.delete<boolean>(`${this.controllerAPIUrl}/transfer/${id}`);
  }
}

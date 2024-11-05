import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { TableLazyLoadEvent } from 'primeng/table';
import { Observable } from 'rxjs';
import { CreditItem } from '../models/credit-item';
import { CreditsResponse } from '../models/credits-response';
import { Payment } from '../models/payment';
import { PaymentInfo } from '../models/payment-info';

@Injectable()
export class CreditsService {
  private controllerAPIUrl: string;

  constructor(private http: HttpClient) {
    this.controllerAPIUrl = `${environment.webapi}/credits`;
  }

  getItems(tableLazyLoadEvent: TableLazyLoadEvent | undefined): Observable<CreditsResponse> {
    return this.http.post<CreditsResponse>(`${this.controllerAPIUrl}/items/`, tableLazyLoadEvent);
  }

  getItem(id: string): Observable<CreditItem> {
    return this.http.get<CreditItem>(`${this.controllerAPIUrl}/${id}`);
  }

  deleteItem(item: CreditItem): Observable<boolean> {
    return this.http.delete<boolean>(`${this.controllerAPIUrl}/${item.idString}`);
  }

  upsertItem(item: CreditItem): Observable<boolean> {
    return this.http.post<boolean>(`${this.controllerAPIUrl}`, item);
  }

  activate(id: string, accountId: string): Observable<boolean> {
    return this.http.post<boolean>(`${this.controllerAPIUrl}/activate/${id}`, {AccountId: accountId});
  }

  getNextPayments(): Observable<PaymentInfo[]> {
    return this.http.get<PaymentInfo[]>(`${this.controllerAPIUrl}/payments/next`);
  }
}

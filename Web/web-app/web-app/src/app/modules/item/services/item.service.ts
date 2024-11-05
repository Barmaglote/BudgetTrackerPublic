import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Item } from '../models';
import { TableLazyLoadEvent } from 'primeng/table';
import { ItemsResponse } from '../interfaces/items-response.interface';
import { Observable } from 'rxjs';

@Injectable()
export class ItemService {
  private controllerAPIUrl: string;

  constructor(private http: HttpClient) {
    this.controllerAPIUrl = `${environment.webapi}`;
  }

  getItems(tableLazyLoadEvent: TableLazyLoadEvent, area: string): Observable<ItemsResponse> {
    return this.http.post<ItemsResponse>(`${this.controllerAPIUrl}/${area}/items/`, tableLazyLoadEvent);
  }

  getItem(id: string, area: string): Observable<Item> {
    return this.http.get<Item>(`${this.controllerAPIUrl}/${area}/${id}`);
  }

  deleteItem(item: Item, area: string): Observable<boolean> {
    return this.http.delete<boolean>(`${this.controllerAPIUrl}/${area}/${item.idString}`);
  }

  upsertItem(item:  Item, area: string, id?: string): Observable<boolean> {
    if (id) {
      return this.http.post<boolean>(`${this.controllerAPIUrl}/${area}?plannedId=${id}`, item);
    } else {
      return this.http.post<boolean>(`${this.controllerAPIUrl}/${area}`, item);
    }
  }
}

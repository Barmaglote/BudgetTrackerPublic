import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { PlannedItem } from 'src/app/shared/models/planned-item';

@Injectable({
  providedIn: 'root'
})
export class PlanningService {
  private controllerAPIUrl: string;

  constructor(private http: HttpClient) {
    this.controllerAPIUrl = `${environment.webapi}/planning`;
  }

  getItems(year?: number, month?: number): Observable<PlannedItem[]> {
    return this.http.get<PlannedItem[]>(`${this.controllerAPIUrl}/${year}/${month}`);
  }

  saveItem(plannedItemView: PlannedItem): Observable<boolean> {
    return this.http.post<boolean>(`${this.controllerAPIUrl}`, plannedItemView);
  }

  deleteItem(item: PlannedItem): Observable<boolean> {
    return this.http.delete<boolean>(`${this.controllerAPIUrl}/${item.idString}`);
  }
}

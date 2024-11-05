import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';

@Injectable()
export class HealthService {
  private controllerAPIUrl: string;

  constructor(private http: HttpClient) {
    this.controllerAPIUrl = `${environment.webapi}/health`;
  }

  getError(): Observable<boolean> {
    return this.http.get<boolean>(`${this.controllerAPIUrl}/error`);
  }

  getState(): Observable<boolean> {
    return this.http.get<boolean>(`${this.controllerAPIUrl}/state`);
  }
}

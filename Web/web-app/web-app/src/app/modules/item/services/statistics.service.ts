import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { TableLazyLoadEvent } from 'primeng/table';
import { StatisticsByCategory } from 'src/app/models/statistics-by-category';
import { StatisticsByDate } from 'src/app/models/statistics-by-date';
import { GeneralCreditsStatistics } from '../../credits/models/general-credits-statistics';
import { BriefStatistics } from '../models/brief-statistics';
import { RegularStatistics } from '../models/regualar-payments';


@Injectable()
export class StatisticsService {
  private controllerAPIUrl: string;

  constructor(private http: HttpClient) {
    this.controllerAPIUrl = `${environment.webapi}/statistics`;
  }

  getStatsByCategory(tableLazyLoadEvent: TableLazyLoadEvent, area: string): Observable<StatisticsByCategory[]> {
    return this.http.post<StatisticsByCategory[]>(`${this.controllerAPIUrl}/${area}/category`, tableLazyLoadEvent);
  }

  getStatsByDate(tableLazyLoadEvent: TableLazyLoadEvent, area: string): Observable<StatisticsByDate[]> {
    return this.http.post<StatisticsByDate[]>(`${this.controllerAPIUrl}/${area}/date`, tableLazyLoadEvent);
  }

  getGeneralCreditsStatistics(): Observable<GeneralCreditsStatistics> {
    return this.http.get<GeneralCreditsStatistics>(`${this.controllerAPIUrl}/credits`);
  }

  getBriefStatistics(): Observable<BriefStatistics> {
    return this.http.get<BriefStatistics>(`${this.controllerAPIUrl}/brief`);
  }

  getRegularPayments(year: number): Observable<RegularStatistics> {
    return this.http.get<RegularStatistics>(`${this.controllerAPIUrl}/regular/${year}`);
  }
}

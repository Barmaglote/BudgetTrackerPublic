import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { UserSettings } from 'src/app/models/user-settings';
import { AccountItem } from 'src/app/shared/models/account-item';

@Injectable({
  providedIn: 'root'
})
export class SettingsService {
  private controllerAPIUrl: string;

  constructor(private http: HttpClient) {
    this.controllerAPIUrl = `${environment.webapi}/usersettings`;
  }

  getUserSettings(): Observable<UserSettings> {
    return this.http.get<UserSettings>(`${this.controllerAPIUrl}`);
  }

  saveUserSettings(userSettings:  UserSettings): Observable<boolean> {
    return this.http.post<boolean>(`${this.controllerAPIUrl}`, userSettings);
  }

  saveAccount(account: AccountItem): Observable<boolean> {
    return this.http.post<boolean>(`${this.controllerAPIUrl}/account`, account);
  }

  deleteAccount(id: string): Observable<boolean> {
    return this.http.delete<boolean>(`${this.controllerAPIUrl}/account/${id}`);
  }

  getAccount(id: string): Observable<AccountItem> {
    return this.http.get<AccountItem>(`${this.controllerAPIUrl}/account/${id}`);
  }

  updateLanguage(language: string): Observable<UserSettings> {
    const options = {
      headers: new HttpHeaders().set('Content-Type', 'application/json'),
    };
    return this.http.post<UserSettings>(`${this.controllerAPIUrl}/language`, '"' + language + '"', options);
  }

  updateLocale(locale: string): Observable<UserSettings> {
    const options = {
      headers: new HttpHeaders().set('Content-Type', 'application/json'),
    };
    return this.http.post<UserSettings>(`${this.controllerAPIUrl}/locale`, '"' + locale + '"', options);
  }
}

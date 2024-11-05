import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

interface ISignResponse {status: string, accessToken: string, refreshToken: string, user: {id: string, login: string, username: string}}

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({ providedIn: 'root' })
export class LoginService {
  private controllerAPIUrl: string;

  constructor(private http: HttpClient) {
    this.controllerAPIUrl = `${environment.loginapi}/api/auth`;
  }

  signUp(title: string, email: string, password: string, recaptcha: string): Observable<boolean> {
    return this.http.post<boolean>(`${this.controllerAPIUrl}/signup/`, {username: title, login: email, password, recaptcha});
  }

  confirm(email: string, token: string | null | undefined): Observable<boolean> {
    return this.http.post<boolean>(`${this.controllerAPIUrl}/confirm/`, {login: email, token});
  }

  signIn(email: string, password: string, recaptcha: string): Observable<ISignResponse> {
    console.log("signIn 01", {login: email, password, recaptcha});
    console.log("signIn 02", `${this.controllerAPIUrl}/signIn/`);
    return this.http.post<ISignResponse>(`${this.controllerAPIUrl}/signIn/`, {login: email, password, recaptcha});
  }

  requestrestore(login: string, recaptcha: string): Observable<{status: string}>{
    return this.http.post<{status: string}>(`${this.controllerAPIUrl}/requestrestore`, {login, recaptcha});
  }

  restore(password:string, token: string, login: string): Observable<{status: string}> {
    return this.http.post<{status: string}>(`${this.controllerAPIUrl}/restore`, {password, token, login});
  }

  refreshtoken(refreshToken: string): Observable<{status: string, accessToken: string}> {
    return this.http.post<{status: string, accessToken: string}>(`${this.controllerAPIUrl}/refreshtoken`, {refreshToken}, httpOptions);
  }

  changePasswordRequest(login: string, oldpassword: string, password: string): Observable<{status: string}> {
    return this.http.post<{status: string}>(`${this.controllerAPIUrl}/changepassword`, {
        login,
        oldpassword,
        password
      });
  }
};


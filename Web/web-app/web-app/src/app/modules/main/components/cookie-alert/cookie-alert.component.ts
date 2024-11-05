import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-cookie-alert',
  templateUrl: './cookie-alert.component.html',
  styleUrls: ['./cookie-alert.component.css']
})
export class CookieAlertComponent implements OnInit {
  hideCookieAlert: boolean = false;

  constructor() { }

  ngOnInit(): void {
    this.checkCookieAlert();
  }

  checkCookieAlert() {
    const hideCookieAlert = localStorage.getItem('hideCookieAlert');
    this.hideCookieAlert = hideCookieAlert ? JSON.parse(hideCookieAlert) : false;
  }

  closeCookieAlert() {
    localStorage.setItem('hideCookieAlert', 'true');
    this.hideCookieAlert = true;
  }
}


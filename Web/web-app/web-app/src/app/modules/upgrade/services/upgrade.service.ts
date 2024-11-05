import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { loadStripe } from '@stripe/stripe-js';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UpgradeService {
  private controllerAPIUrl: string;
  private stripePromise = loadStripe('pk_test_51PTQSyRoj8McxcxjIccGQXMCfSy7Ujn8IDEUk8FixdNyBTKInVy1TYlELSxiuml7VcV11vOBCBqKDCMe2OzE7o3500iA7DUUNJ');

  constructor(private http: HttpClient) {
    this.controllerAPIUrl = `${environment.paymentapi}/api`;
  }

  createStripeCheckoutSession(priceId: string): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(`${this.controllerAPIUrl}/stripe/session`, { priceId });
  }

  createStripePaymentIntent(amount: number, currency: string, userId: string, planId: string, email: string): Observable<{ clientSecret: string }> {
    return this.http.post<{ clientSecret: string }>(`${this.controllerAPIUrl}/stripe/intent`, { amount, currency, userId, planId, email });
  }

  subscribe(planId: string, paymentMethodId: string, email: string, userId: string, provider: string): Observable<boolean> {
    return this.http.post<boolean>(`${this.controllerAPIUrl}/stripe/subscribe`, { planId, paymentMethodId, email, userId, provider });
  }

  unsubscribe(userId: string, subscribtionId: string, provider: string): Observable<boolean> {
    return this.http.post<boolean>(`${this.controllerAPIUrl}/stripe/unsubscribe`, { subscribtionId, userId, provider });
  }

  async loadStripe() {
    return await this.stripePromise;
  }
}

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { PaymentIntent } from '@stripe/stripe-js';

@Injectable()
export class PaymentService {
  private controllerAPIUrl: string;

  constructor(private http: HttpClient) {
    this.controllerAPIUrl = `${environment.webapi}/payment`;
  }

  sendPaymentResultToBackend(paymentIntent: PaymentIntent): Observable<boolean> {

    const paymentIntentWithDateTime: PaymentIntentWithDateTime = {
      ...paymentIntent,
      created: new Date(paymentIntent.created * 1000).toISOString(),
      paymentMethodTypes: paymentIntent.payment_method_types,
      clientSecret: paymentIntent.client_secret,
      captureMethod: paymentIntent.capture_method,
      confirmationMethod: paymentIntent.confirmation_method
    };

    return this.http.post<boolean>(`${this.controllerAPIUrl}/result`, {paymentIntent: paymentIntentWithDateTime});
  }
}


export interface PaymentIntentWithDateTime extends Omit<PaymentIntent, 'created'> {
  created: string;
  paymentMethodTypes: any,
  clientSecret: any,
  captureMethod: any,
  confirmationMethod: any,
}

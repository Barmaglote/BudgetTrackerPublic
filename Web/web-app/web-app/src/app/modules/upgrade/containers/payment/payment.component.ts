import { Component, OnInit } from '@angular/core';
import { Stripe, StripeElements } from '@stripe/stripe-js';
import { UpgradeService } from '../../services';
import { Store } from '@ngrx/store';
import * as featureSettingsStore from '../../../settings/store';
import * as featureStore from '../../store';
import { Observable, firstValueFrom } from 'rxjs';
import { UserSettings } from 'src/app/models/user-settings';
import { PaymentPlan } from '../../models/payment-plan';
import { AppError } from 'src/app/core/models';
import { MessageService } from 'primeng/api';
import { AuthenticationService } from 'src/app/core/services';

@Component({
  selector: 'app-payment',
  templateUrl: './payment.component.html',
  styleUrls: ['./payment.component.css']
})
export class PaymentComponent implements OnInit {
  public userSettings$: Observable<UserSettings | undefined> = this.store.select(featureSettingsStore.SettingsSelectors.userSettings);
  public plan$: Observable<PaymentPlan | undefined> = this.store.select(featureStore.UpgradeSelectors.plan);
  stripe: Stripe | null = null;
  elements: StripeElements | null | undefined = null;
  card: any;
  clientSecret: string | null = null;
  public isVisible = false;

  style = {
    base: {
      color: '#32325d',
      fontFamily: '"Helvetica Neue", Helvetica, sans-serif',
      fontSmoothing: 'antialiased',
      fontSize: '16px',
      '::placeholder': {
        color: '#aab7c4'
      }
    },
    invalid: {
      color: '#fa755a',
      iconColor: '#fa755a'
    },
    complete: {
      color: '#4caf50',
      iconColor: '#4caf50'
    }
  };

  constructor(
    private stripeService: UpgradeService,
    private authenticationService: AuthenticationService,
    private store: Store,
    private messageService: MessageService) { }
  async ngOnInit() {
    this.stripe = await this.stripeService.loadStripe();
    this.elements = this.stripe?.elements();
    this.card = this.elements?.create('card', { style: this.style });
    this.card.mount('#card-element');
    const userSettings = await firstValueFrom(this.userSettings$);
    const plan = await firstValueFrom(this.plan$);
    if (!userSettings) {
      this.messageService.clear();
      this.messageService.add({ ...new AppError("Failed to get user settings"), life: 2000 });
      return;
    }

    if (!plan) {
      this.messageService.clear();
      this.messageService.add({ ...new AppError("Failed to get plan"), life: 2000 });
      return;
    }

    this.stripeService.createStripePaymentIntent(plan.price, plan.currency, userSettings.idString, plan.planId, this.authenticationService.getUser()?.email).subscribe({
      next: (response) => {
        this.clientSecret = response.clientSecret;
        this.isVisible = true;
      },
      error: (error) => {
        console.error('Error creating PaymentIntent', error);
        throw error;
      }
    });
  }

  async onSubmit() {
    if (!this.stripe || !this.clientSecret || !this.card) {
      console.error('Stripe.js has not loaded or clientSecret is missing');
      return;
    }

    const plan = await firstValueFrom(this.plan$);

    const { error, paymentIntent } = await this.stripe.confirmCardPayment(this.clientSecret, {
      payment_method: {
        card: this.card,
      }
    });

    if (error) {
      console.error('Payment failed:', error);
      this.store.dispatch(featureStore.UpgradeActions.paymentIsFailed());
    } else {
      console.log('Payment succeeded:', paymentIntent);
      console.log('USER', this.authenticationService.getUser())
      this.store.dispatch(featureStore.UpgradeActions.paymentIsSuccessfull(plan?.planId || '', paymentIntent.payment_method?.toString() || '', this.authenticationService.getUser()?.email, this.authenticationService.getUser()?.id, this.authenticationService.getUser()?.provider));
    }
  }
  onCancel() {
    this.store.dispatch(featureStore.UpgradeActions.navigateToRoot());
  }
}

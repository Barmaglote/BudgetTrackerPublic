import { createActionGroup, emptyProps } from '@ngrx/store';
import { PaymentPlan } from '../models/payment-plan';

export const UpgradeActions  = createActionGroup({
  source: 'Upgrade',
  events: {
    'Clean Store': emptyProps(),
    'Navigate To Payment': (plan: PaymentPlan) => ({plan}),
    'Navigate To Root': emptyProps(),
    'Payment Is Successfull': (planId: string, paymentMethodId: string, email: string, userId: string, provider: string) => ({planId, paymentMethodId, email, userId, provider}),
    'Payment Is Failed': emptyProps(),
    'Unsubscribe': (subscribtionId: string, userId: string, provider: string) => ({subscribtionId, userId, provider}),
  },
});

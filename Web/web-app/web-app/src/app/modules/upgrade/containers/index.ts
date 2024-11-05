import { PaymentFailedComponent } from "./payment-failed/payment-failed.component";
import { PaymentSuccessComponent } from "./payment-success/payment-success.component";
import { PaymentComponent } from "./payment/payment.component";
import { UnsubscribeComponent } from "./unsubscribe/unsubscribe.component";
import { UpgradeRootComponent } from "./upgrade-root/upgrade-root.component";

export const UPGRADE_CONTAINERS = [
  UpgradeRootComponent,
  PaymentSuccessComponent,
  PaymentFailedComponent,
  PaymentComponent,
  UnsubscribeComponent
]

export * from "./upgrade-root/upgrade-root.component";
export * from "./payment-failed/payment-failed.component";
export * from "./payment-success/payment-success.component";
export * from "./payment/payment.component";
export * from "./unsubscribe/unsubscribe.component";

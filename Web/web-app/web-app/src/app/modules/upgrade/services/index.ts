import { PaymentService } from "./payment.service";
import { UpgradeService } from "./upgrade.service";

export const UPGRADE_SERVICES = [
  UpgradeService,
  PaymentService
];

export * from "./upgrade.service";
export * from "./payment.service";

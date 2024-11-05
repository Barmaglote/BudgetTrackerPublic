import { Component, Input } from '@angular/core';
import { PaymentPlan } from '../../models/payment-plan';

@Component({
  selector: 'app-plan-info',
  templateUrl: './plan-info.component.html',
  styleUrls: ['./plan-info.component.css']
})
export class PlanInfoComponent {
  @Input() plan: PaymentPlan | undefined | null = undefined;
}

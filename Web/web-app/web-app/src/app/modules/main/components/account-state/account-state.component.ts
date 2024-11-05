import { SocialUser } from '@abacritt/angularx-social-login';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-account-state',
  templateUrl: './account-state.component.html',
  styleUrls: ['./account-state.component.css']
})
export class AccountStateComponent {
  @Input() user: SocialUser | undefined;
  @Input() income: number = 0;
  @Input() expenses: number = 0;
  @Input() incomeCurrency: string = '';
  @Input() expensesCurrency: string = '';
}

import { Component, EventEmitter, Inject, Input, LOCALE_ID, OnDestroy, OnInit, Output, forwardRef } from '@angular/core';
import { BehaviorSubject, Subscription } from 'rxjs';
import { AccountItem } from 'src/app/shared/models/account-item';
import { CreditItem } from '../../models/credit-item';
import { Category } from 'src/app/models/category';
import { FormBuilder, FormGroup, NG_VALUE_ACCESSOR, Validators } from '@angular/forms';
import { categoryValidator } from 'src/app/modules/item/models/category-validator';
import { CreditCalculator } from '../../models';
import { Payment } from '../../models/payment';
import { GarbageCollector } from 'src/app/models';

@Component({
  selector: 'app-add-credit',
  templateUrl: './add-credit.component.html',
  styleUrls: ['./add-credit.component.css'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AddCreditComponent),
      multi: true,
    },
  ],
})
export class AddCreditComponent implements OnInit, OnDestroy {
  @Input({required : true}) categories: Category[] | undefined;
  @Input({required: true}) accounts: AccountItem[] = [];
  @Output() add = new EventEmitter<CreditItem>();
  public payments$: BehaviorSubject<Payment[] | undefined> = new BehaviorSubject<Payment[] | undefined>(undefined);
  public repayment$: BehaviorSubject<number> = new BehaviorSubject<number>(0);
  public interestPaid$: BehaviorSubject<number> = new BehaviorSubject<number>(0);
  public autoUpdate: boolean = true;
  private plan: Payment[] | undefined;

  public selectedAccount$: BehaviorSubject<AccountItem | null> = new BehaviorSubject<AccountItem | null>(null);

  public newItemForm: FormGroup = this.formBuilder.group({
    account: [null, [Validators.required]],
    category: [null, [Validators.required, categoryValidator()]],
    date: [null, [Validators.required]],
    comment: [''],
    quantity: [0, [Validators.required, Validators.min(1)]],
    months: [0, [Validators.required, Validators.min(1)]],
    rate: [0, [Validators.required, Validators.min(0.1)]],
    mandatory: [0],
    isIncluded: [false],
  });

  private _newItemFormSubscription: Subscription | undefined;
  private _paymentsSubscription: Subscription | undefined;

  constructor(private formBuilder: FormBuilder, @Inject(LOCALE_ID) public locale: string) {
  }
  ngOnDestroy(): void {
    const garbageCollector = new GarbageCollector([
      this.payments$,
      this.repayment$,
      this.interestPaid$,
      this.selectedAccount$
    ]);
    garbageCollector.collect();

    if (this._newItemFormSubscription) {
      this._newItemFormSubscription.unsubscribe();
    }

    if (this._paymentsSubscription) {
      this._paymentsSubscription.unsubscribe();
    }
  }

  ngOnInit(): void {
    this._newItemFormSubscription = this.newItemForm.valueChanges.subscribe((value) => {
      if (!this.autoUpdate) {
        return;
      }
      let {date, comment, months, rate, quantity, category, account, mandatory, isIncluded} = value;
      let creditItem = {date, comment, months, rate, quantity, category: category?.category, accountId: account.id, mandatory, isIncluded} as CreditItem
      this.payments$.next(this.getPaymentSchedule(creditItem));
      this.repayment$.next(this.getTotalRepayment(creditItem));
      this.interestPaid$.next(this.getTotalInterestPaid(creditItem));
    });

    this._paymentsSubscription = this.payments$.subscribe(plan => {
      if (!this.autoUpdate) {
        return;
      }

      this.plan = plan ? JSON.parse(JSON.stringify(plan)) : [];
    });
  }

  onSubmit(){
    let {date, comment, months, rate, quantity, category, account, mandatory, isIncluded} = this.newItemForm.value;

    if (this.newItemForm.invalid) {
      this.newItemForm.markAllAsTouched();
      return;
    }

    this.add.emit({date, comment, months, rate, quantity, category: category?.category, accountId: account.id, mandatory, isIncluded, plan: this.plan} as CreditItem);
  }

  onAccountChange(event: any){
    this.selectedAccount$.next(event.value);
  }

  public getPaymentSchedule(creditItem: CreditItem) {
    const {quantity, rate, mandatory, isIncluded, months, date} = creditItem;

    const calculator = new CreditCalculator(
      quantity,
      rate,
      mandatory,
      isIncluded,
      months,
      date
    );

    return calculator.getPaymentSchedule();
  }

  public getTotalRepayment(creditItem: CreditItem) {
    const {quantity, rate, mandatory, isIncluded, months, date} = creditItem;

    const calculator = new CreditCalculator(
      quantity,
      rate,
      mandatory,
      isIncluded,
      months,
      date
    );

    return calculator.getTotalRepayment();
  }


  public getTotalInterestPaid(creditItem: CreditItem) {
    const {quantity, rate, mandatory, isIncluded, months, date} = creditItem;

    const calculator = new CreditCalculator(
      quantity,
      rate,
      mandatory,
      isIncluded,
      months,
      date
    );

    return calculator.getTotalInterestPaid();
  }

  onPlanUpdate(plan: Payment[] | undefined) {
    this.plan = plan;
  }
}

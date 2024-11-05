import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { PlannedItem } from 'src/app/shared/models/planned-item';

@Component({
  templateUrl: './add-planned-item.component.html',
  styleUrls: ['./add-planned-item.component.css']
})
export class AddPlannedItemComponent implements OnInit {
  public newItemForm: FormGroup = this.formBuilder.group({
    quantity: [0, [Validators.required, Validators.min(1)]],
    comment: [null, [Validators.required]],
    area: [null, [Validators.required]],
    date: [null, [Validators.required]],
  });

  public currency: string | null = null;
  public idString: string | null = null;

  constructor(private formBuilder: FormBuilder, public ref: DynamicDialogRef, public config: DynamicDialogConfig) { }

  ngOnInit(): void {
    if (this.config.data?.plannedItem) {
      var { currency, quantity, comment, idString, date, area} = this.config.data.plannedItem;

      this.currency = currency;
      this.idString = idString;

      this.newItemForm.setValue({ quantity, comment, area, date: date ? new Date(date) : new Date()});
    }
  }

  onSubmit() {
    let {quantity, comment, date, area} = this.newItemForm.value;

    if (this.newItemForm.invalid) {
      this.newItemForm.markAllAsTouched();
      return;
    }

    this.ref.close((this.idString ? {quantity, comment, area, date, currency: this.currency, idString: this.idString } : {quantity, area, comment, date, currency: this.currency }) as PlannedItem);
  }

  onCurrencyChange(currency: string) {
    this.currency = currency;
  }
}

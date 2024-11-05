import { Component, Input } from '@angular/core';
import { RowInfoAggrigated } from '../../models/row-info-aggrigated';

@Component({
  selector: 'app-aggrigated-items',
  templateUrl: './aggrigated-items.component.html',
  styleUrls: ['./aggrigated-items.component.css']
})
export class AggrigatedItemsComponent {
  @Input() items: RowInfoAggrigated[] | [] = []
  @Input() header: string = '';
}

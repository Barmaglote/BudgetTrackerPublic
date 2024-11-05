import { Component, EventEmitter, Input, Output } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Item } from 'src/app/modules/item/models';

@Component({
  selector: 'app-items-list',
  templateUrl: './items-list.component.html',
  styleUrls: ['./items-list.component.scss']
})
export class ItemsListComponent {
  @Input() income$: Observable<Item[] | undefined> = new BehaviorSubject<Item[] | undefined>([]);
  @Input() expenses$: Observable<Item[] | undefined> = new BehaviorSubject<Item[] | undefined>([]);
  @Input() currency: string | undefined;
  @Output() onLoad = new EventEmitter<string>();
}

import { Component, ElementRef, Input, TemplateRef } from '@angular/core';

@Component({
  selector: 'app-card',
  templateUrl: './card.component.html',
  styleUrls: ['./card.component.css']
})
export class CardComponent {
  @Input() header: string | undefined = undefined;
  @Input() headerTemplate: TemplateRef<ElementRef> | undefined;
}

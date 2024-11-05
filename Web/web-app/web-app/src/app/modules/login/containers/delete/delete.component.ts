import { Component } from '@angular/core';
import { Location } from '@angular/common';

@Component({
  selector: 'app-delete',
  templateUrl: './delete.component.html',
  styleUrls: ['./delete.component.css']
})
export class DeleteComponent{

  constructor(private location: Location) { }

  backClicked() {
    this.location.back();
  }
}

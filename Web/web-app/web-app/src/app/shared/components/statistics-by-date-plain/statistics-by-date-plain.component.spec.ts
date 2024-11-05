/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { StatisticsByDatePlainComponent } from './statistics-by-date-plain.component';

describe('StatisticsByDatePlainComponent', () => {
  let component: StatisticsByDatePlainComponent;
  let fixture: ComponentFixture<StatisticsByDatePlainComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StatisticsByDatePlainComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StatisticsByDatePlainComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

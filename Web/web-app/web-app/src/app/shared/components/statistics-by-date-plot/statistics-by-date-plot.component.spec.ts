/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { StatisticsByDatePlotComponent } from './statistics-by-date-plot.component';

describe('StatisticsByDatePlotComponent', () => {
  let component: StatisticsByDatePlotComponent;
  let fixture: ComponentFixture<StatisticsByDatePlotComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StatisticsByDatePlotComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StatisticsByDatePlotComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

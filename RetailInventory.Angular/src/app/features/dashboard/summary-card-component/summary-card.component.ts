import { Component, input } from '@angular/core';

import { SummaryMetric } from '../models/dashboard.models';

@Component({
  selector: 'app-summary-card-component',
  templateUrl: './summary-card.component.html',
})
export class SummaryCardComponent {
  readonly metric = input.required<SummaryMetric>();
}

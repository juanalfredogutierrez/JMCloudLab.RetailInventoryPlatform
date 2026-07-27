import { Component, input } from '@angular/core';

import { AccountDistribution } from '../models/dashboard.models';

@Component({
  selector: 'app-distribution-card-component',
  templateUrl: './distribution-card.component.html',
})
export class DistributionCardComponent {
  readonly distribution = input.required<AccountDistribution[]>();
}

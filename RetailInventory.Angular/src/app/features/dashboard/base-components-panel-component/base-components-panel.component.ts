import { Component, input } from '@angular/core';

import { TableTransaction } from '../models/dashboard.models';

@Component({
  selector: 'app-base-components-panel-component',
  templateUrl: './base-components-panel.component.html',
})
export class BaseComponentsPanelComponent {
  readonly tableTransactions = input.required<TableTransaction[]>();
}

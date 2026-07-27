import { Component, input } from '@angular/core';

import { Transaction } from '../models/dashboard.models';

@Component({
  selector: 'app-transactions-list-component',
  templateUrl: './transactions-list.component.html',
})
export class TransactionsListComponent {
  readonly transactions = input.required<Transaction[]>();
}

import { Component, signal } from '@angular/core';

import { BaseComponentsPanelComponent } from '../base-components-panel-component/base-components-panel.component';
import { DistributionCardComponent } from '../distribution-card-component/distribution-card.component';
import {
  AccountDistribution,
  NotificationItem,
  SummaryMetric,
  TableTransaction,
  Transaction,
} from '../models/dashboard.models';
import { NotificationsCardComponent } from '../notifications-card-component/notifications-card.component';
import { SummaryCardComponent } from '../summary-card-component/summary-card.component';
import { TransactionsListComponent } from '../transactions-list-component/transactions-list.component';

@Component({
  selector: 'app-dashboard-page-component',
  imports: [
    SummaryCardComponent,
    TransactionsListComponent,
    DistributionCardComponent,
    NotificationsCardComponent,
    BaseComponentsPanelComponent,
  ],
  templateUrl: './dashboard-page.component.html',
})
export class DashboardPageComponent {
  protected readonly summaryMetrics = signal<SummaryMetric[]>([
    {
      title: 'Productos',
      value: '120',
      description: 'Registrados',
      tone: 'positive',
      icon: '📦',
      iconClass: 'green',
      cardClass: 'green-card',
      sparklinePath: 'M2 44 C28 25 50 31 70 25 S96 1 124 12 S145 26 168 8',
    },
    {
      title: 'Compras hoy',
      value: '15',
      description: 'Órdenes registradas',
      tone: 'neutral',
      icon: '🛒',
      iconClass: 'blue',
    },
    {
      title: 'Ventas Hoy',
      value: '36',
      description: 'Transacciones realizadas',
      tone: 'positive',
      icon: '💰',
      iconClass: 'purple',
      cardClass: 'purple-card',
      lineClass: 'purple-line',
      sparklinePath: 'M2 42 C22 34 28 14 48 22 S74 37 93 21 S118 18 134 8 S154 9 168 0',
    },
    {
      title: 'Stock Bajo',
      value: '24',
      description: 'Productos por reabastecer',
      tone: 'negative',
      icon: '⚠️',
      iconClass: 'orange',
      cardClass: 'orange-card',
      lineClass: 'orange-line',
      sparklinePath: 'M2 44 C22 33 24 16 47 26 S76 34 89 14 S122 9 139 16 S155 6 168 2',
    },
  ]);

protected readonly transactions = signal<Transaction[]>([
  {
    icon: '📥',
    iconClass: 'green',
    title: 'Compra COM-001',
    date: 'Hoy, 09:45 AM',
    account: 'Ingreso de stock',
    amount: '+ 100 unidades',
    tone: 'positive',
  },
  {
    icon: '📤',
    iconClass: 'blue',
    title: 'Venta VEN-001',
    date: 'Hoy, 11:00 AM',
    account: 'Salida de stock',
    amount: '- 10 unidades',
    tone: 'negative',
  },
  {
    icon: '📥',
    iconClass: 'green',
    title: 'Compra COM-002',
    date: 'Ayer',
    account: 'Ingreso de stock',
    amount: '+ 50 unidades',
    tone: 'positive',
  }
]);

protected readonly distribution = signal<AccountDistribution[]>([
  {
    colorClass: 'blue',
    label: 'Electrónica',
    amount: '45 productos',
    percent: '35%'
  },
  {
    colorClass: 'teal',
    label: 'Accesorios',
    amount: '38 productos',
    percent: '30%'
  },
  {
    colorClass: 'purple',
    label: 'Periféricos',
    amount: '25 productos',
    percent: '19%'
  },
  {
    colorClass: 'orange',
    label: 'Otros',
    amount: '19 productos',
    percent: '16%'
  }
]);
protected readonly notifications = signal<NotificationItem[]>([
  {
    icon: '⚠',
    iconClass: 'orange',
    title: 'Producto Mouse Gamer con stock bajo',
    date: 'Hoy'
  },
  {
    icon: 'ℹ',
    iconClass: 'blue',
    title: 'Nueva compra registrada COM-001',
    date: 'Hoy'
  }
]);

protected readonly tableTransactions = signal<TableTransaction[]>([
  {
    date: '07/06/2026',
    description: 'Compra COM-001',
    amount: '+100',
    tone: 'positive'
  },
  {
    date: '07/06/2026',
    description: 'Venta VEN-001',
    amount: '-10',
    tone: 'negative'
  },
  {
    date: '08/06/2026',
    description: 'Compra COM-002',
    amount: '+50',
    tone: 'positive'
  }
]);
}

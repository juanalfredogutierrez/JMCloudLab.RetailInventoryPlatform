import { Component, input } from '@angular/core';

import { NotificationItem } from '../models/dashboard.models';

@Component({
  selector: 'app-notifications-card-component',
  templateUrl: './notifications-card.component.html',
})
export class NotificationsCardComponent {
  readonly notifications = input.required<NotificationItem[]>();
}

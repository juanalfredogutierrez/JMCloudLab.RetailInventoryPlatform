export type TrendTone = 'positive' | 'negative' | 'neutral';

export interface SummaryMetric {
  title: string;
  value: string;
  description: string;
  tone: TrendTone;
  icon: string;
  iconClass: string;
  cardClass?: string;
  lineClass?: string;
  sparklinePath?: string;
}

export interface Transaction {
  icon: string;
  iconClass: string;
  title: string;
  date: string;
  account: string;
  amount: string;
  tone: TrendTone;
}

export interface AccountDistribution {
  colorClass: string;
  label: string;
  amount: string;
  percent: string;
}

export interface NotificationItem {
  icon: string;
  iconClass: string;
  title: string;
  date: string;
}

export interface TableTransaction {
  date: string;
  description: string;
  amount: string;
  tone: TrendTone;
}

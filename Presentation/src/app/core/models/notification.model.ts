export interface Notification {
  id: string;
  senderName: string;
  message: string;
  status: NotificationStatus;
  type: NotificationType;
  priority: NotificationPriority;
  referenceId?: string;
  referenceType?: string;
  createdAt: Date;
}

export enum NotificationStatus {
  Unread = 'Unread',
  Read = 'Read',
  Archived = 'Archived'
}

export enum NotificationType {
  Payment = 'Payment',
  Contract = 'Contract',
  PropertyUpdate = 'PropertyUpdate',
  Visit = 'Visit',
  Proposal = 'Proposal',
  Negotiation = 'Negotiation'
}

export enum NotificationPriority {
  Low = 'Low',
  Medium = 'Medium',
  High = 'High'
}


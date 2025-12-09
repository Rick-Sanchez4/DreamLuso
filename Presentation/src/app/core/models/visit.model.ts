export interface PropertyVisit {
  id: string;
  propertyId: string;
  propertyTitle?: string;
  propertyAddress?: string;
  clientId: string;
  clientName?: string;
  realEstateAgentId: string;
  agentName?: string;
  visitDate: string; // DateOnly format: YYYY-MM-DD
  timeSlot: string; // TimeSlot enum string
  status: VisitStatus;
  notes?: string;
  clientFeedback?: string;
  clientRating?: number;
  confirmationToken?: string;
  confirmedAt?: Date;
  cancelledAt?: Date;
  cancellationReason?: string;
  createdAt: Date;
}

export enum VisitStatus {
  Pending = 'Pending',
  Confirmed = 'Confirmed',
  Completed = 'Completed',
  Cancelled = 'Cancelled',
  Rescheduled = 'Rescheduled',
  NoShow = 'NoShow'
}

export interface ScheduleVisitRequest {
  propertyId: string;
  clientId: string;
  realEstateAgentId: string;
  visitDate: string; // DateOnly format: YYYY-MM-DD
  timeSlot: number; // TimeSlot enum value (0-4)
  notes?: string;
}

export enum TimeSlot {
  Morning_9AM_11AM = 0,
  Morning_11AM_1PM = 1,
  Afternoon_2PM_4PM = 2,
  Afternoon_4PM_6PM = 3,
  Evening_6PM_8PM = 4
}

export interface ConfirmVisitRequest {
  visitId: string;
}

export interface CancelVisitRequest {
  visitId: string;
  reason: string;
}

export interface AvailableTimeSlot {
  date: Date;
  startTime: string;
  endTime: string;
  isAvailable: boolean;
}


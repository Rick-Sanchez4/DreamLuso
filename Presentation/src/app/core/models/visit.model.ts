export interface PropertyVisit {
  id: string;
  propertyId: string;
  propertyTitle?: string;
  clientId: string;
  clientName?: string;
  realEstateAgentId: string;
  agentName?: string;
  scheduledDate: Date;
  duration: number; // in minutes
  status: VisitStatus;
  notes?: string;
  feedback?: string;
  createdAt: Date;
}

export enum VisitStatus {
  Pending = 'Pending',
  Confirmed = 'Confirmed',
  Completed = 'Completed',
  Cancelled = 'Cancelled',
  NoShow = 'NoShow'
}

export interface ScheduleVisitRequest {
  propertyId: string;
  clientId: string;
  realEstateAgentId: string;
  scheduledDate: Date;
  duration: number;
  notes?: string;
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


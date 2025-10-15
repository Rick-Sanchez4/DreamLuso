export interface PropertyProposal {
  id: string;
  proposalNumber: string;
  propertyId: string;
  propertyTitle: string;
  clientId: string;
  clientName: string;
  proposedValue: number;
  type: ProposalType;
  status: ProposalStatus;
  paymentMethod?: string;
  intendedMoveDate?: Date;
  additionalNotes?: string;
  createdAt: Date;
  negotiations: ProposalNegotiation[];
}

export interface ProposalNegotiation {
  id: string;
  senderName: string;
  message: string;
  counterOffer?: number;
  status: NegotiationStatus;
  sentAt: Date;
}

export enum ProposalType {
  Purchase = 'Purchase',
  Rent = 'Rent'
}

export enum ProposalStatus {
  Pending = 'Pending',
  UnderAnalysis = 'UnderAnalysis',
  InNegotiation = 'InNegotiation',
  Approved = 'Approved',
  Rejected = 'Rejected',
  Cancelled = 'Cancelled'
}

export enum NegotiationStatus {
  Sent = 'Sent',
  Viewed = 'Viewed',
  Accepted = 'Accepted',
  Rejected = 'Rejected'
}

export interface CreateProposalRequest {
  propertyId: string;
  clientId: string;
  proposedValue: number;
  type: ProposalType;
  paymentMethod?: string;
  intendedMoveDate?: Date;
  additionalNotes?: string;
}


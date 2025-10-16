export interface Property {
  id: string;
  title: string;
  description: string;
  propertyType: PropertyType;
  transactionType: TransactionType;
  status: PropertyStatus;
  price: number;
  area: number;
  bedrooms: number;
  bathrooms: number;
  address: Address;
  // Address fields (from API response)
  street?: string;
  municipality?: string;
  district?: string;
  postalCode?: string;
  images: PropertyImage[];
  realEstateAgentId: string;
  agentName?: string;
  averageRating?: number;
  totalComments?: number;
  createdAt: Date;
}

export interface Address {
  street: string;
  city: string;
  district: string;
  postalCode: string;
  country: string;
  latitude?: number;
  longitude?: number;
}

export interface PropertyImage {
  id: string;
  url: string;
  isMain: boolean;
}

export enum PropertyType {
  Apartment = 'Apartment',
  House = 'House',
  Villa = 'Villa',
  Land = 'Land',
  Commercial = 'Commercial'
}

export enum TransactionType {
  Sale = 'Sale',
  Rent = 'Rent'
}

export enum PropertyStatus {
  Available = 0,
  Reserved = 1,
  UnderContract = 2,
  Sold = 3,
  Rented = 4,
  Unavailable = 5,
  InNegotiation = 6,
  Inactive = 5  // Map to Unavailable
}

export interface PropertySearchFilters {
  city?: string;
  minPrice?: number;
  maxPrice?: number;
  propertyType?: PropertyType;
  transactionType?: TransactionType;
  minBedrooms?: number;
  minBathrooms?: number;
}


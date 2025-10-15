export interface Comment {
  id: string;
  userId: string;
  userName: string;
  message: string;
  rating?: number;
  helpfulCount: number;
  createdAt: Date;
  replies: Comment[];
}

export interface PropertyRating {
  averageRating: number;
  totalComments: number;
  fiveStars: number;
  fourStars: number;
  threeStars: number;
  twoStars: number;
  oneStar: number;
}

export interface CreateCommentRequest {
  propertyId: string;
  userId: string;
  message: string;
  rating?: number;
  parentCommentId?: string;
}


import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Comment, PropertyRating, CreateCommentRequest } from '../models/comment.model';
import { Result } from '../models/result.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CommentService {
  private readonly apiUrl = `${environment.apiUrl}/comments`;

  constructor(private http: HttpClient) {}

  create(comment: CreateCommentRequest): Observable<Result<string>> {
    return this.http.post<Result<string>>(this.apiUrl, comment);
  }

  getPropertyComments(propertyId: string): Observable<Result<Comment[]>> {
    return this.http.get<Result<Comment[]>>(`${this.apiUrl}/property/${propertyId}`);
  }

  getPropertyRating(propertyId: string): Observable<Result<PropertyRating>> {
    return this.http.get<Result<PropertyRating>>(`${this.apiUrl}/property/${propertyId}/rating`);
  }

  incrementHelpful(commentId: string): Observable<Result<boolean>> {
    return this.http.put<Result<boolean>>(`${this.apiUrl}/${commentId}/helpful`, {});
  }

  flagComment(commentId: string): Observable<Result<boolean>> {
    return this.http.put<Result<boolean>>(`${this.apiUrl}/${commentId}/flag`, {});
  }

  deleteComment(commentId: string): Observable<Result<boolean>> {
    return this.http.delete<Result<boolean>>(`${this.apiUrl}/${commentId}`);
  }
}


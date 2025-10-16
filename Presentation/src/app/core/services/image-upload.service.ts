import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface UploadImagesResponse {
  urls: string[];
  count: number;
}

@Injectable({
  providedIn: 'root'
})
export class ImageUploadService {
  private readonly apiUrl = `${environment.apiUrl}/images`;

  constructor(private http: HttpClient) {}

  uploadImages(files: File[]): Observable<UploadImagesResponse> {
    const formData = new FormData();
    
    files.forEach((file, index) => {
      formData.append('files', file, file.name);
    });

    return this.http.post<UploadImagesResponse>(`${this.apiUrl}/upload`, formData);
  }
}


import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment.development';
import { Category } from '../models/category';
import { CreateCategoryRequest } from '../models/create-category-request';
import { UpdateCategoryRequest } from '../models/update-category-request';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/categories`;

  getCategories(includeInactive = false): Observable<Category[]> {
    const params = new HttpParams().set(
      'includeInactive',
      includeInactive.toString()
    );

    return this.http.get<Category[]>(this.apiUrl, { params });
  }

  getCategoryById(id: number): Observable<Category> {
    return this.http.get<Category>(`${this.apiUrl}/${id}`);
  }

  createCategory(
    request: CreateCategoryRequest
  ): Observable<Category> {
    return this.http.post<Category>(this.apiUrl, request);
  }

  updateCategory(
    id: number,
    request: UpdateCategoryRequest
  ): Observable<void> {
    return this.http.put<void>(
      `${this.apiUrl}/${id}`,
      request
    );
  }

  deactivateCategory(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}

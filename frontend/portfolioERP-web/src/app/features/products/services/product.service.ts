import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment.development';
import { PagedResponse } from '../../../shared/models/paged-response';
import { Product } from '../models/product';
import { ProductQuery } from '../models/product-query';
import { CreateProductRequest } from '../models/create-product-request';
import { UpdateProductRequest } from '../models/update-product-request';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/products`;

  getProducts(query: ProductQuery): Observable<PagedResponse<Product>> {
    let params = new HttpParams();

    if (query.search?.trim()) {
      params = params.set('search', query.search.trim());
    }

    if (query.categoryId !== undefined) {
      params = params.set('categoryId', query.categoryId.toString());
    }

    if (query.isActive !== undefined) {
      params = params.set('isActive', query.isActive.toString());
    }

    if (query.minPrice !== undefined) {
      params = params.set('minPrice', query.minPrice.toString());
    }

    if (query.maxPrice !== undefined) {
      params = params.set('maxPrice', query.maxPrice.toString());
    }

    params = params
      .set('sortBy', query.sortBy ?? 'name')
      .set('descending', (query.descending ?? false).toString())
      .set('pageNumber', (query.pageNumber ?? 1).toString())
      .set('pageSize', (query.pageSize ?? 10).toString());

    return this.http.get<PagedResponse<Product>>(
      this.apiUrl,
      { params }
    );
  }

  getProductById(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/${id}`);
  }

  createProduct(request: CreateProductRequest): Observable<Product> {
    return this.http.post<Product>(this.apiUrl, request);
  }

  updateProduct(
    id: number,
    request: UpdateProductRequest
  ): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, request);
  }

  deactivateProduct(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getActiveProducts(): Observable<PagedResponse<Product>> {
    return this.getProducts({
      isActive: true,
      sortBy: 'name',
      descending: false,
      pageNumber: 1,
      pageSize: 100
    });
  }
}

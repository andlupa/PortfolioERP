import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';
import { PagedResponse } from '../../../shared/models/paged-response';
import { CreateSupplierRequest } from '../models/create-supplier-request';
import { Supplier } from '../models/supplier';
import { SupplierQuery } from '../models/supplier-query';
import { UpdateSupplierRequest } from '../models/update-supplier-request';

@Injectable({
  providedIn: 'root'
})
export class SupplierService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl =
    `${environment.apiUrl}/suppliers`;

  getSuppliers(
    query: SupplierQuery
  ): Observable<PagedResponse<Supplier>> {
    let params = new HttpParams();

    if (query.search?.trim()) {
      params = params.set(
        'search',
        query.search.trim()
      );
    }

    if (query.city?.trim()) {
      params = params.set(
        'city',
        query.city.trim()
      );
    }

    if (query.country?.trim()) {
      params = params.set(
        'country',
        query.country.trim()
      );
    }

    if (query.isActive !== undefined) {
      params = params.set(
        'isActive',
        query.isActive.toString()
      );
    }

    params = params
      .set('sortBy', query.sortBy ?? 'companyName')
      .set(
        'descending',
        (query.descending ?? false).toString()
      )
      .set(
        'pageNumber',
        (query.pageNumber ?? 1).toString()
      )
      .set(
        'pageSize',
        (query.pageSize ?? 10).toString()
      );

    return this.http.get<PagedResponse<Supplier>>(
      this.apiUrl,
      { params }
    );
  }

  getById(id: number): Observable<Supplier> {
    return this.http.get<Supplier>(
      `${this.apiUrl}/${id}`
    );
  }

  create(
    request: CreateSupplierRequest
  ): Observable<Supplier> {
    return this.http.post<Supplier>(
      this.apiUrl,
      request
    );
  }

  update(
    id: number,
    request: UpdateSupplierRequest
  ): Observable<Supplier> {
    return this.http.put<Supplier>(
      `${this.apiUrl}/${id}`,
      request
    );
  }

  deactivate(id: number): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${id}`
    );
  }
}

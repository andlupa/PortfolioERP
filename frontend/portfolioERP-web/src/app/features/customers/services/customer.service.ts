import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment.development';
import { PagedResponse } from '../../../shared/models/paged-response';
import { CreateCustomerRequest } from '../models/create-customer-request';
import { Customer } from '../models/customer';
import { CustomerQuery } from '../models/customer-query';
import { UpdateCustomerRequest } from '../models/update-customer-request';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/customers`;

  getCustomers(
    query: CustomerQuery
  ): Observable<PagedResponse<Customer>> {
    let params = new HttpParams();

    if (query.search?.trim()) {
      params = params.set('search', query.search.trim());
    }

    if (query.city?.trim()) {
      params = params.set('city', query.city.trim());
    }

    if (query.country?.trim()) {
      params = params.set('country', query.country.trim());
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

    return this.http.get<PagedResponse<Customer>>(
      this.apiUrl,
      { params }
    );
  }

  getCustomerById(id: number): Observable<Customer> {
    return this.http.get<Customer>(`${this.apiUrl}/${id}`);
  }

  createCustomer(
    request: CreateCustomerRequest
  ): Observable<Customer> {
    return this.http.post<Customer>(this.apiUrl, request);
  }

  updateCustomer(
    id: number,
    request: UpdateCustomerRequest
  ): Observable<void> {
    return this.http.put<void>(
      `${this.apiUrl}/${id}`,
      request
    );
  }

  deactivateCustomer(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getActiveCustomers(): Observable<PagedResponse<Customer>> {
    return this.getCustomers({
      isActive: true,
      sortBy: 'companyName',
      descending: false,
      pageNumber: 1,
      pageSize: 100
    });
  }
}

import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';
import { PagedResponse } from '../../../shared/models/paged-response';
import { OrderListItem } from '../models/order-list-item';
import { OrderQuery } from '../models/order-query';

import { CreateOrderRequest } from '../models/create-order-request';
import { OrderResponse } from '../models/order-response';
import { CalculateOrderRequest } from '../models/calculate-order-request';
import { OrderCalculationResponse } from '../models/order-calculation-response';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/orders`;

  getOrders(
    query: OrderQuery
  ): Observable<PagedResponse<OrderListItem>> {
    let params = new HttpParams();

    if (query.search?.trim()) {
      params = params.set('search', query.search.trim());
    }

    if (query.customerId !== undefined) {
      params = params.set(
        'customerId',
        query.customerId.toString()
      );
    }

    if (query.status !== undefined) {
      params = params.set(
        'status',
        query.status.toString()
      );
    }

    if (query.dateFrom) {
      params = params.set('dateFrom', query.dateFrom);
    }

    if (query.dateTo) {
      params = params.set('dateTo', query.dateTo);
    }

    if (query.minTotal !== undefined) {
      params = params.set(
        'minTotal',
        query.minTotal.toString()
      );
    }

    if (query.maxTotal !== undefined) {
      params = params.set(
        'maxTotal',
        query.maxTotal.toString()
      );
    }

    params = params
      .set('sortBy', query.sortBy ?? 'orderDate')
      .set(
        'descending',
        (query.descending ?? true).toString()
      )
      .set(
        'pageNumber',
        (query.pageNumber ?? 1).toString()
      )
      .set(
        'pageSize',
        (query.pageSize ?? 10).toString()
      );

    return this.http.get<PagedResponse<OrderListItem>>(
      this.apiUrl,
      { params }
    );
  }

  getOrderById(id: number): Observable<OrderResponse> {
    return this.http.get<OrderResponse>(`${this.apiUrl}/${id}`);
  }

  createOrder(
    request: CreateOrderRequest
  ): Observable<OrderResponse> {
    return this.http.post<OrderResponse>(
      this.apiUrl,
      request
    );
  }

  confirmOrder(id: number): Observable<void> {
    return this.http.post<void>(
      `${this.apiUrl}/${id}/confirm`,
      {}
    );
  }

  cancelOrder(id: number): Observable<void> {
    return this.http.post<void>(
      `${this.apiUrl}/${id}/cancel`,
      {}
    );
  }

  calculateOrder(
    request: CalculateOrderRequest
  ): Observable<OrderCalculationResponse> {
    return this.http.post<OrderCalculationResponse>(
      `${this.apiUrl}/calculate`,
      request
    );
  }

  ship(id: number): Observable<void> {
    return this.http.post<void>(
      `${this.apiUrl}/${id}/ship`,
      {}
    );
  }

}

import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment.development';
import { CreatePurchaseOrderRequest } from '../models/create-purchase-order-request';
import { PurchaseOrder } from '../models/purchase-order';
import { PurchaseOrderListItem } from '../models/purchase-order-list-item';

@Injectable({
  providedIn: 'root'
})
export class PurchaseOrderService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    `${environment.apiUrl}/purchaseorders`;

  getAll(): Observable<PurchaseOrderListItem[]> {
    return this.http.get<PurchaseOrderListItem[]>(
      this.apiUrl
    );
  }

  getById(id: number): Observable<PurchaseOrder> {
    return this.http.get<PurchaseOrder>(
      `${this.apiUrl}/${id}`
    );
  }

  create(
    request: CreatePurchaseOrderRequest
  ): Observable<PurchaseOrder> {
    return this.http.post<PurchaseOrder>(
      this.apiUrl,
      request
    );
  }

  markAsOrdered(id: number): Observable<PurchaseOrder> {
    return this.http.post<PurchaseOrder>(
      `${this.apiUrl}/${id}/order`,
      {}
    );
  }

  receive(id: number): Observable<PurchaseOrder> {
    return this.http.post<PurchaseOrder>(
      `${this.apiUrl}/${id}/receive`,
      {}
    );
  }

  cancel(id: number): Observable<PurchaseOrder> {
    return this.http.post<PurchaseOrder>(
      `${this.apiUrl}/${id}/cancel`,
      {}
    );
  }
}

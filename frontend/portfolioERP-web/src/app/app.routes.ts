import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { adminGuard } from './core/guards/admin.guard';

export const routes: Routes = [
  {
    path: '',
    canActivate: [ authGuard ],
    loadComponent: () =>
      import('./core/layout/main-layout/main-layout')
        .then(component => component.MainLayout),
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        loadComponent: () =>
          import(
            './features/dashboard/pages/dashboard/dashboard'
          ).then(component => component.Dashboard)
      },
      {
        path: 'products/new',
        loadComponent: () =>
          import(
            './features/products/pages/product-form-page/product-form-page'
          ).then(component => component.ProductFormPage)
      },
      {
        path: 'products/:id/edit',
        loadComponent: () =>
          import(
            './features/products/pages/product-form-page/product-form-page'
          ).then(component => component.ProductFormPage)
      },
      {
        path: 'products',
        loadComponent: () =>
          import(
            './features/products/pages/product-list-page/product-list-page'
          ).then(component => component.ProductListPage)
      },
      {
        path: 'suppliers/new',
        loadComponent: () =>
          import(
            './features/suppliers/pages/supplier-form-page/supplier-form-page'
          ).then(
            component =>
              component.SupplierFormPage
          )
      },
      {
        path: 'suppliers/:id/edit',
        loadComponent: () =>
          import(
            './features/suppliers/pages/supplier-form-page/supplier-form-page'
          ).then(
            component =>
              component.SupplierFormPage
          )
      },
      {
        path: 'suppliers',
        loadComponent: () =>
          import(
            './features/suppliers/pages/supplier-list-page/supplier-list-page'
          ).then(
            component =>
              component.SupplierListPage
          )
      },
      {
        path: 'purchase-orders',
        loadComponent: () =>
          import(
            './features/purchase-orders/pages/purchase-order-list-page/purchase-order-list-page'
          ).then(
            component =>
              component.PurchaseOrderListPage
          )
      },
      {
        path: 'purchase-orders/new',
        loadComponent: () =>
          import(
            './features/purchase-orders/pages/purchase-order-form-page/purchase-order-form-page'
          ).then(
            component =>
              component.PurchaseOrderFormPage
          )
      },
      {
        path: 'purchase-orders/:id',
        loadComponent: () =>
          import(
            './features/purchase-orders/pages/purchase-order-detail-page/purchase-order-detail-page'
          ).then(
            component =>
              component.PurchaseOrderDetailPage
          )
      },
      {
        path: 'categories/new',
        loadComponent: () =>
          import(
            './features/categories/pages/category-form-page/category-form-page'
          ).then(component => component.CategoryFormPage)
      },
      {
        path: 'categories/:id/edit',
        loadComponent: () =>
          import(
            './features/categories/pages/category-form-page/category-form-page'
          ).then(component => component.CategoryFormPage)
      },
      {
        path: 'categories',
        loadComponent: () =>
          import(
            './features/categories/pages/category-list-page/category-list-page'
          ).then(component => component.CategoryListPage)
      },
      {
        path: 'customers/new',
        loadComponent: () =>
          import(
            './features/customers/pages/customer-form-page/customer-form-page'
          ).then(component => component.CustomerFormPage)
      },
      {
        path: 'customers/:id/edit',
        loadComponent: () =>
          import(
            './features/customers/pages/customer-form-page/customer-form-page'
          ).then(component => component.CustomerFormPage)
      },
      {
        path: 'customers',
        loadComponent: () =>
          import(
            './features/customers/pages/customer-list-page/customer-list-page'
          ).then(component => component.CustomerListPage)
      },
      {
        path: 'orders/new',
        loadComponent: () =>
          import(
            './features/orders/pages/order-form-page/order-form-page'
          ).then(component => component.OrderFormPage)
      },
      {
        path: 'orders/:id',
        loadComponent: () =>
          import(
            './features/orders/pages/order-detail-page/order-detail-page'
          ).then(component => component.OrderDetailPage)
      },
      {
        path: 'orders',
        loadComponent: () =>
          import(
            './features/orders/pages/order-list-page/order-list-page'
          ).then(component => component.OrderListPage)
      },
      {
        path: 'users/new',
        canActivate: [adminGuard],

        loadComponent: () =>
          import(
            './features/users/pages/user-form-page/user-form-page'
          ).then(
            component =>
              component.UserFormPage
          )
      },
      {
        path: 'users',
        canActivate: [adminGuard],

        loadComponent: () =>
          import(
            './features/users/pages/user-list-page/user-list-page'
          ).then(
            component =>
              component.UserListPage
          )
      }
    ]
  },
  {
    path: 'dashboard',
    loadComponent: () =>
      import(
        './features/dashboard/pages/dashboard/dashboard'
      ).then(component => component.Dashboard)
  },
  {
    path: 'login',
    loadComponent: () =>
      import(
        './features/auth/pages/login-page/login-page'
      ).then(
        component =>
          component.LoginPage
      )
  },
  {
    path: '**',
    redirectTo: 'products'
  }
];

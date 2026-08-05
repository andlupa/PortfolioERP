# Wharehouse_BackEnd
A modern ERP web application developed with ASP.NET Core Web API, Angular, PostgreSQL and Docker, showcasing enterprise-level architecture, REST APIs, authentication, and business workflows.

# PortfolioERP

PortfolioERP is a full-stack Enterprise Resource Planning (ERP) application developed as a professional portfolio project to demonstrate modern software development practices using the Microsoft technology stack.

The project simulates a real-world business management system and showcases backend architecture, RESTful API design, database modeling, frontend development, authentication, and deployment.

---

## Technologies

### Backend

- ASP.NET Core (.NET 9)
- C#
- Entity Framework Core
- LINQ
- RESTful APIs
- JWT Authentication
- Swagger / OpenAPI

### Frontend

- Angular
- TypeScript
- RxJS
- Angular Material

### Database

- PostgreSQL

### DevOps

- Docker
- Docker Compose
- GitHub

---

## Planned Features

### Authentication

- JWT Authentication
- Refresh Tokens
- Role-Based Authorization

### User Management

- Users
- Roles
- Permissions

### Product Management

- Categories
- Products
- Inventory

### Customer Management

- Customers
- Contacts

### Sales

- Orders
- Order Lines
- Order Status

### Purchasing

- Suppliers
- Purchase Orders

### Reporting

- Dashboard
- Charts
- Business KPIs

---

## Architecture

The solution follows a modular Clean Architecture approach.

```
PortfolioERP
│
├── PortfolioERP.Api
├── PortfolioERP.Application
├── PortfolioERP.Domain
├── PortfolioERP.Infrastructure
└── PortfolioERP.Tests
```

### Main Principles

- Separation of Concerns
- Dependency Injection
- SOLID Principles
- Repository Pattern
- DTO Pattern
- Clean Architecture
- RESTful Design

---

## REST API

Example endpoints

```
GET    /api/products
GET    /api/products/{id}

POST   /api/products

PUT    /api/products/{id}

DELETE /api/products/{id}
```

---

## Project Goals

The purpose of this project is to demonstrate practical experience in:

- ASP.NET Core
- Angular
- PostgreSQL
- Entity Framework Core
- REST API Design
- Authentication & Authorization
- Software Architecture
- Clean Code
- Enterprise Development

---

## Future Improvements

- Docker deployment
- Microservices
- RabbitMQ
- Redis Cache
- Background Jobs
- Unit Tests
- Integration Tests
- CI/CD with GitHub Actions
- Kubernetes deployment

---

## Status

🚧 Work in Progress

This project is actively being developed.

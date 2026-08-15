# PortfolioERP

PortfolioERP is a full-stack Enterprise Resource Planning (ERP) application developed as a professional portfolio project to demonstrate the design, development and deployment of a modern enterprise web application.

The project simulates a real-world business management system and covers the complete application lifecycle: database design, backend architecture, REST API development, authentication and authorization, Angular frontend development, containerization and automated cloud deployment.

## 🌐 Live Demo

**Frontend:**  
https://portfolioerp.pages.dev
User: demo
Password: DemoPwd0

The Angular frontend is hosted on Cloudflare Pages and communicates with an ASP.NET Core REST API running on Microsoft Azure.

---

## ✨ Features

### Authentication & Security

- JWT Bearer Authentication
- Password hashing
- Role-based authorization
- Protected API endpoints
- Angular HTTP authentication interceptor
- CORS configuration
- Secure production configuration using Azure Container Apps secrets

### Dashboard

- Business overview
- Sales information
- Purchasing information
- Operational KPIs

### Product Management

- Categories CRUD
- Products CRUD
- Search and filtering
- Sorting
- Pagination
- VAT management
- Stock management
- Product activation/deactivation

### Customer Management

- Customers CRUD
- Customer data validation

### Sales

- Sales orders
- Sales order lines
- Automatic amount calculation
- VAT and discounts
- Stock validation and updates
- Order confirmation and cancellation
- Transactional order creation

### Purchasing

- Suppliers CRUD
- Purchase orders
- Purchase order lines
- VAT and discount calculation
- Purchasing workflow

### Application Infrastructure

- Global exception handling
- Request validation with FluentValidation
- Structured logging with Serilog
- Entity Framework Core migrations
- Dependency Injection
- DTO-based API contracts

---

## 🛠 Technology Stack

### Backend

- C#
- ASP.NET Core
- Entity Framework Core
- LINQ
- RESTful APIs
- FluentValidation
- Serilog
- JWT Bearer Authentication
- Swagger / OpenAPI
- Docker

### Frontend

- Angular
- TypeScript
- RxJS
- Angular Signals
- Reactive Forms
- Bootstrap

### Database

- Microsoft SQL Server
- Azure SQL Database
- Entity Framework Core migrations

### Cloud & DevOps

- Microsoft Azure
- Azure Container Apps
- Azure SQL Database
- Cloudflare Pages
- Docker
- Docker Compose
- GitHub
- GitHub Actions
- GitHub Container Registry (GHCR)
- OpenID Connect (OIDC)

---

## 🏗 Architecture

PortfolioERP follows a layered architecture with clear separation between domain logic, application contracts, infrastructure and API concerns.

```text
PortfolioERP
│
├── backend
│   ├── PortfolioERP.Api
│   ├── PortfolioERP.Application
│   ├── PortfolioERP.Domain
│   ├── PortfolioERP.Infrastructure
│   └── PortfolioERP.Tests
│
├── frontend
│   └── portfolioERP-web
│
├── .github
│   └── workflows
│
├── Dockerfile
└── compose.yaml
```

### Main Principles

- Separation of Concerns
- Dependency Injection
- SOLID principles
- DTO pattern
- Layered / Clean Architecture principles
- RESTful API design
- Asynchronous programming
- Centralized validation and error handling

---

## ☁️ Production Architecture

```text
                    GitHub
                      │
          ┌───────────┴───────────┐
          │                       │
          ▼                       ▼
   GitHub Actions          Cloudflare Pages
          │                       │
     Build & Test              Angular SPA
          │                       │
     Docker Build                  │
          │                       │
          ▼                       │
         GHCR                      │
          │                       │
          ▼                       │
 Azure Container Apps ◄───────────┘
          │                 HTTPS / REST
          ▼
    ASP.NET Core API
          │
     Entity Framework Core
          │
          ▼
     Azure SQL Database
```

---

## 🔐 Authentication

PortfolioERP uses JWT Bearer authentication.

The authentication flow is:

```text
Angular
   │
   │ POST /api/auth/login
   ▼
ASP.NET Core API
   │
   │ Validate credentials
   ▼
JWT Token
   │
   │ Authorization: Bearer <token>
   ▼
Protected REST API endpoints
```

Angular automatically attaches the JWT to protected HTTP requests using an HTTP interceptor.

---

## 🔄 CI/CD Pipeline

Every push to the `main` branch triggers the backend CI/CD pipeline using GitHub Actions.

The pipeline:

1. Restores .NET dependencies
2. Builds the solution
3. Runs automated tests
4. Builds the Docker image
5. Pushes the image to GitHub Container Registry
6. Authenticates to Azure using OpenID Connect
7. Deploys the new image to Azure Container Apps
8. Creates a new application revision

```text
git push
    │
    ▼
GitHub Actions
    │
    ├── Restore
    ├── Build
    ├── Test
    ├── Docker Build
    ├── Push to GHCR
    ├── Azure OIDC Login
    │
    ▼
Azure Container Apps
```

The deployment uses **OIDC federation**, so no long-lived Azure credentials are stored in GitHub.

Cloudflare Pages independently detects frontend changes in the GitHub repository, builds the Angular application and deploys the new frontend version.

---

## 🌐 REST API

Examples of available endpoints:

```text
POST   /api/auth/login

GET    /api/dashboard

GET    /api/categories
POST   /api/categories
PUT    /api/categories/{id}
DELETE /api/categories/{id}

GET    /api/products
GET    /api/products/{id}
POST   /api/products
PUT    /api/products/{id}

GET    /api/customers
POST   /api/customers
PUT    /api/customers/{id}

GET    /api/orders
POST   /api/orders

GET    /api/suppliers
POST   /api/suppliers

GET    /api/purchaseorders
POST   /api/purchaseorders
```

Protected endpoints require:

```http
Authorization: Bearer <JWT_TOKEN>
```

---

## 🐳 Docker

The ASP.NET Core backend is containerized using Docker.

The same application image can run locally or in Azure Container Apps, while environment-specific configuration and credentials are supplied externally.

This keeps production secrets out of the source code and Docker image.

---

## 🔒 Security

The project implements several production-oriented security practices:

- Passwords stored as hashes
- JWT authentication
- Role-based authorization
- Restricted CORS origins
- Production secrets stored outside source control
- Azure Container Apps secret references
- GitHub Actions authentication through OIDC
- No Azure passwords stored in the CI/CD pipeline

---

## 🎯 Project Goals

PortfolioERP demonstrates practical experience with:

- Full-stack enterprise application development
- C# and ASP.NET Core
- Angular and TypeScript
- REST API design
- Relational database modeling
- Entity Framework Core
- Authentication and authorization
- Docker containerization
- Microsoft Azure
- CI/CD automation
- Cloud deployment
- Software architecture
- Clean Code and SOLID principles

---

## 🚀 Future Improvements

Planned improvements include:

- Increased unit test coverage
- Integration tests
- Refresh token support
- Advanced role and permission management
- Reporting and charts
- Audit logging
- Background jobs
- Redis caching
- Improved monitoring and observability

Possible architectural experiments:

- RabbitMQ
- Microservices
- Kubernetes

---

## 📌 Status

**Deployed and operational.**

PortfolioERP is actively being developed and extended as a professional full-stack portfolio project.
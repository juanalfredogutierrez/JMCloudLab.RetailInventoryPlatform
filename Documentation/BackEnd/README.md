# RetailInventory.Microservices

Enterprise Inventory Management Platform built with .NET 8 using a Microservices Architecture, CQRS, Event-Driven Communication, RabbitMQ, Docker, and SQL Server.

The solution demonstrates modern backend engineering practices including Clean Architecture, Outbox Pattern, Inbox Pattern, Distributed Trace Propagation, JWT Authentication, API Gateway, and asynchronous messaging.

---

# Overview

RetailInventory is a sample enterprise-grade inventory platform designed to showcase modern software architecture patterns and microservices communication strategies.

The platform manages:

* Product Catalog
* Purchases
* Sales
* Inventory Management
* Authentication & Authorization

The solution emphasizes scalability, reliability, observability, and maintainability.

---

# Architecture

```text
                           ┌─────────────────────┐
                           │   Angular Frontend  │
                           └──────────┬──────────┘
                                      │
                                      ▼
                           ┌─────────────────────┐
                           │ Ocelot API Gateway  │
                           └──────────┬──────────┘
                                      │
        ┌─────────────────────────────┼─────────────────────────────┐
        │                             │                             │
        ▼                             ▼                             ▼

 ┌─────────────┐              ┌─────────────┐             ┌─────────────┐
 │ AuthService │              │ProductoSvc  │             │TransaccionSvc│
 └─────────────┘              └─────────────┘             └──────┬──────┘
                                                                  │
                                                                  │
                                                         Outbox Pattern
                                                                  │
                                                                  ▼

                                                           ┌──────────┐
                                                           │ RabbitMQ │
                                                           └────┬─────┘
                                                                │
                                                                ▼

                                                        Inbox Pattern
                                                                │
                                                                ▼

                                                      ┌────────────────┐
                                                      │InventarioSvc   │
                                                      └────────────────┘

```

---

# Implemented Architecture Patterns

## Clean Architecture

```text
API
Application
Domain
Infrastructure
```

Responsibilities are clearly separated to improve maintainability, testability, and scalability.

---

## CQRS

Command Query Responsibility Segregation implemented using MediatR.

### Commands

```text
CreateProductoCommand
CreateCompraCommand
CreateVentaCommand
RegistrarEntradaCommand
RegistrarSalidaCommand
```

### Queries

```text
GetProductosQuery
GetInventarioQuery
GetStockQuery
```

---

## Event-Driven Architecture

Services communicate asynchronously through RabbitMQ events.

Examples:

```text
compra.registrada
venta.registrada
```

---

## Outbox Pattern

Guarantees reliable event publication.

Events are first persisted in the database and later published asynchronously by a dedicated background worker.

Benefits:

* No event loss
* Better resilience
* Eventual consistency
* Reliable integration

---

## Inbox Pattern

Provides idempotent event processing.

Each processed event is stored and validated before execution to avoid duplicate processing.

Benefits:

* Safe retries
* Duplicate message protection
* Reliable consumer behavior

---

## Distributed Trace Propagation

Trace identifiers are propagated between microservices and integration events.

Benefits:

* End-to-end request tracing
* Easier troubleshooting
* Improved observability

---

# Technologies

### Backend

* .NET 8
* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* MediatR
* FluentValidation

### Messaging

* RabbitMQ
* Outbox Pattern
* Inbox Pattern

### Security

* JWT Authentication
* Role-Based Authorization
* ## Authentication

JWT tokens include the following claims:

- sub
- unique_name
- email
- role

### Infrastructure

* Docker
* Docker Compose
* Ocelot API Gateway

### Testing

* xUnit
* FluentAssertions
* Moq

---

# Microservices

## AuthService

Responsible for:

* User Authentication
* JWT Token Generation
* Role Management

---

## ProductoService

Responsible for:

* Product Registration
* Product Management
* Product Queries

---

## TransaccionService

Responsible for:

* Purchase Registration
* Sales Registration
* Event Publication
* Outbox Management

---

## InventarioService

Responsible for:

* Inventory Control
* Stock Updates
* Event Consumption
* Inbox Processing

---

# Purchase Flow

```text
Client
   │
   ▼

TransaccionService
   │
   ▼

Save Purchase
   │
   ▼

OutboxMessage
   │
   ▼

OutboxPublisherWorker
   │
   ▼

RabbitMQ
   │
   ▼

InventarioService
   │
   ▼

Inbox Validation
   │
   ▼

Inventory Update
```

---

# Sales Flow

```text
Client
   │
   ▼

TransaccionService
   │
   ▼

Stock Validation
   │
   ▼

Save Sale
   │
   ▼

OutboxMessage
   │
   ▼

RabbitMQ
   │
   ▼

InventarioService
   │
   ▼

Inbox Validation
   │
   ▼

Inventory Deduction
```

---

# Docker Support

The entire platform can be executed using Docker Compose.

Included services:

* API Gateway
* AuthService
* ProductoService
* TransaccionService
* InventarioService
* SQL Server
* RabbitMQ

### Start

```bash
docker compose -f docker-compose.infrastructure.yml up -d

docker compose -f docker-compose.services.yml up -d
```

### Stop

```bash
docker compose down
```

---

# Features

* JWT Authentication
* API Gateway
* CQRS
* FluentValidation
* Result Pattern
* RabbitMQ Integration
* Outbox Pattern
* Inbox Pattern
* Distributed Trace Propagation
* Dockerized Infrastructure
* Automatic EF Core Migrations
* Automatic Seed Data
* Unit Testing
* Centralized BuildingBlocks
* Serilog + Seq

---

# Current Releases

```text
v1.0.0 Initial Stable Release
v1.0.1 Validation Pipeline Refactor
v1.1.0 Dockerized Microservices Platform
v1.2.0 Outbox Pattern & Distributed Trace Propagation
```

---

# Future Improvements


* OpenTelemetry
* Distributed Metrics
* Dead Letter Queue (DLQ)
* Retry Policies
* Kubernetes Deployment
* GitHub Actions CI/CD
* Integration Tests
* Contract-First APIs

---

# Author

Juan Gutierrez

Senior Software Engineer | Technical Leader

.NET • Azure • Microservices • DDD • CQRS • Event-Driven Architecture

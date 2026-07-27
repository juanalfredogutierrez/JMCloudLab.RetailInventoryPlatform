# 🏪 JMCloudLab Retail Inventory Platform

> Enterprise Retail Inventory Platform built with **.NET 8**, **Microservices**, **Clean Architecture**, **DDD**, **CQRS**, **RabbitMQ**, **Docker**, **OpenTelemetry**, **Jaeger**, **Seq**, **SQL Server** and **Angular**.

---

## 📌 Overview

JMCloudLab Retail Inventory Platform is an enterprise-inspired inventory management platform designed using modern software architecture principles.

The solution demonstrates how to build scalable, observable and maintainable distributed systems using .NET 8 Microservices, Domain-Driven Design (DDD), CQRS, Clean Architecture and Event-Driven Communication.

This project is intended as a **portfolio project** and a **reference implementation** for enterprise-grade backend development.

---

# ✨ Features

- ✅ .NET 8 Microservices
- ✅ Clean Architecture
- ✅ Domain Driven Design (DDD)
- ✅ CQRS + MediatR
- ✅ Entity Framework Core
- ✅ SQL Server
- ✅ RabbitMQ Messaging
- ✅ Event-Driven Architecture
- ✅ API Gateway (Ocelot)
- ✅ Docker & Docker Compose
- ✅ OpenTelemetry
- ✅ Distributed Tracing
- ✅ Jaeger Integration
- ✅ Structured Logging
- ✅ Seq Logging
- ✅ CorrelationId
- ✅ Global Exception Handling
- ✅ FluentValidation
- ✅ Swagger Documentation
- ✅ Angular Frontend
- ✅ Enterprise Monorepo

---

# 🏗 Solution Architecture

```
                                   +-----------------------+
                                   |      Angular UI       |
                                   +-----------+-----------+
                                               |
                                               |
                                      HTTP / JWT
                                               |
                                               ▼
                                 +-------------------------+
                                 |     Ocelot Gateway      |
                                 +-----------+-------------+
                                             |
          -----------------------------------------------------------------
          |                     |                    |                     |
          ▼                     ▼                    ▼                     ▼
+----------------+     +----------------+    +----------------+    +----------------+
|  Auth Service  |     | ProductoSvc    |    | InventarioSvc  |    |TransaccionSvc |
+----------------+     +----------------+    +----------------+    +----------------+
          |                     |                    |                     |
          ---------------------------------------------------------------
                                   |
                              RabbitMQ Events
                                   |
          ---------------------------------------------------------------
                                   |
                              SQL Server Databases

                    OpenTelemetry + Jaeger + Seq
```

---

# 📁 Solution Structure

```
RetailInventory
│
├── .github/
│
├── Documentation/
│
├── Database/
│
├── Infrastructure/
│   ├── Azure/
│   ├── Docker/
│   ├── Helm/
│   ├── Kubernetes/
│   └── Scripts/
│
├── Observability/
│
├── RetailInventory.Angular/
│
└── RetailInventory.Microservices/
    ├── Gateway.OcelotGateway/
    ├── AuthService/
    ├── ProductoService/
    ├── InventarioService/
    ├── TransaccionService/
    │
    ├── BuildingBlocks/
    ├── BuildingBlocks.Resilience/
    ├── BuildingBlocks.OpenTelemetry/
    ├── BuildingBlocks.Observability/
    ├── BuildingBlocks.HealthChecks/
    └── BuildingBlocks.Messaging/
```

---

# 🧱 Technology Stack

| Category | Technologies |
|-----------|--------------|
| Backend | .NET 8, ASP.NET Core |
| Frontend | Angular |
| Architecture | Clean Architecture |
| Design Patterns | DDD, CQRS, Repository, Dependency Injection |
| Messaging | RabbitMQ |
| Database | SQL Server |
| ORM | Entity Framework Core |
| Validation | FluentValidation |
| Logging | Serilog |
| Observability | OpenTelemetry |
| Tracing | Jaeger |
| Log Storage | Seq |
| API Gateway | Ocelot |
| Documentation | Swagger |
| Containers | Docker |
| Orchestration | Docker Compose |
| Future | Kubernetes, Helm, Azure |

---

# 🚀 Getting Started

## Clone Repository

```bash
git clone https://github.com/juanalfredogutierrez/JMCloudLab.RetailInventoryPlatform.git
```

```
cd JMCloudLab.RetailInventoryPlatform
```

---

## Run using Docker

```
cd Infrastructure/Docker

docker compose up -d --build
```

---

# 🌐 Available Services

| Service | URL |
|----------|-----|
| Gateway | http://localhost:5000 |
| Auth Service | http://localhost:5001 |
| Producto Service | http://localhost:5002 |
| Transacción Service | http://localhost:5003 |
| Inventario Service | http://localhost:5004 |
| Seq | http://localhost:5341 |
| Jaeger | http://localhost:16686 |
| RabbitMQ Management | http://localhost:15672 |

---

# 🔍 Observability

This project includes a complete observability stack.

## Logging

- Serilog
- Structured Logging
- CorrelationId
- Request Logging
- Business Logging

## Distributed Tracing

- OpenTelemetry
- Jaeger
- TraceId propagation
- Span hierarchy

## Metrics

- OpenTelemetry Metrics

---

# 📷 Screenshots

## Swagger

> *(Add Screenshot)*

---

## Docker Desktop

> *(Add Screenshot)*

---

## Jaeger

> *(Add Screenshot)*

---

## Seq

> *(Add Screenshot)*

---

## RabbitMQ

> *(Add Screenshot)*

---

# 📦 Microservices

## Auth Service

Responsible for:

- Authentication
- JWT Token generation
- User management

---

## Producto Service

Responsible for:

- Product Catalog
- Product Pricing
- Product Queries

---

## Inventario Service

Responsible for:

- Inventory Management
- Stock
- Inventory Events

---

## Transaccion Service

Responsible for:

- Purchases
- Sales
- Outbox Pattern
- Integration Events

---

# 📡 Event Driven Communication

Current Events

- CompraRegistrada
- VentaRegistrada
- ActualizarCostoProducto

RabbitMQ is used as the asynchronous communication broker.

---

# 📈 Roadmap

## Completed

- [x] .NET 8
- [x] Docker
- [x] Docker Compose
- [x] RabbitMQ
- [x] SQL Server
- [x] Clean Architecture
- [x] DDD
- [x] CQRS
- [x] MediatR
- [x] OpenTelemetry
- [x] Jaeger
- [x] Seq
- [x] API Gateway
- [x] Angular Frontend

## Next Steps

- [ ] Redis
- [ ] Health Checks UI
- [ ] Kubernetes
- [ ] Helm Charts
- [ ] Azure AKS
- [ ] GitHub Actions CI/CD
- [ ] SonarQube
- [ ] Prometheus
- [ ] Grafana
- [ ] API Versioning
- [ ] Identity Server
- [ ] Rate Limiting

---

# 🤝 Contributing

Contributions are welcome.

Please fork the repository and submit a Pull Request.

---

# 📄 License

This project is licensed under the MIT License.

---

# 👨‍💻 Author

**Juan Alfredo Gutierrez**

Senior .NET Developer

- Backend Architecture
- Clean Architecture
- DDD
- CQRS
- Microservices
- Azure
- Docker
- Kubernetes
- OpenTelemetry

GitHub

https://github.com/juanalfredogutierrez

LinkedIn

https://www.linkedin.com/in/juanalfredogutierrez/

---

# ⭐ If you found this project useful...

v2.0

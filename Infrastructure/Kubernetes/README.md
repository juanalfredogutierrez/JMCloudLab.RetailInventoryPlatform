# Kubernetes

This folder contains all Kubernetes manifests required to deploy the JMCloudLab Retail Inventory Platform.

---

## Structure

base/
Shared Kubernetes resources

infrastructure/
Infrastructure components

applications/
Application workloads

ingress/
Ingress Controllers

scripts/
Automation scripts

manifests/
Deployment order

---

## Requirements

Docker Desktop Kubernetes

kubectl

---

## Deployment Order

1 Namespace

2 Secrets

3 ConfigMaps

4 Storage

5 SQL Server

6 RabbitMQ

7 Seq

8 Jaeger

9 Auth Service

10 Producto Service

11 Inventario Service

12 Transaccion Service

13 Gateway

14 Frontend

15 Ingress
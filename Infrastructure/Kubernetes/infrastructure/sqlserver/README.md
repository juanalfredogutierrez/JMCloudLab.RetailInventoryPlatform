# SQL Server

## Descripción

Este componente despliega SQL Server 2022 Developer Edition utilizado por la plataforma RetailInventory.

---

## Recursos

| Recurso | Archivo |
|---------|----------|
| Deployment | deployment.yaml |
| Service | service.yaml |
| Secret | secret.yaml |
| PersistentVolumeClaim | pvc.yaml |

---

## Imagen

```text
mcr.microsoft.com/mssql/server:2022-latest
```

---

## Puerto

```
1433/TCP
```

---

## Persistencia

PVC

```
10Gi
```

StorageClass

```
standard
```

---

## Secret

```
sqlserver-secret
```

Variables

```
SA_PASSWORD
```

---

## Service

```
ClusterIP
```

Nombre DNS

```
sqlserver
```

---

## Despliegue

```bash
kubectl apply -k .
```

---

## Eliminación

```bash
kubectl delete -k .
```
Write-Host ""
Write-Host "========== NAMESPACES =========="
kubectl get ns

Write-Host ""
Write-Host "========== PODS =========="
kubectl get pods -n retailinventory

Write-Host ""
Write-Host "========== SERVICES =========="
kubectl get svc -n retailinventory

Write-Host ""
Write-Host "========== DEPLOYMENTS =========="
kubectl get deployment -n retailinventory

Write-Host ""
Write-Host "========== PVC =========="
kubectl get pvc -n retailinventory
Write-Host ""
Write-Host "========================================="
Write-Host " Destroying Platform"
Write-Host "========================================="
Write-Host ""

kubectl delete -k ../applications --ignore-not-found

kubectl delete -k ../infrastructure --ignore-not-found

kubectl delete -k ../base --ignore-not-found

Write-Host ""
Write-Host "Platform removed."
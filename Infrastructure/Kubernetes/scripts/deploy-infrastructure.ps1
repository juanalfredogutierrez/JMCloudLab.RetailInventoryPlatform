Write-Host ""
Write-Host "========================================="
Write-Host " Deploying Infrastructure"
Write-Host "========================================="
Write-Host ""

kubectl apply -k ../infrastructure

Write-Host ""
Write-Host "Infrastructure deployed successfully."
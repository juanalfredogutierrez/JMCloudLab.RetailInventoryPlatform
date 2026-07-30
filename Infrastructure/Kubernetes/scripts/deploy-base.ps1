Write-Host ""
Write-Host "========================================="
Write-Host " Deploying Applications"
Write-Host "========================================="
Write-Host ""

kubectl apply -k ../applications

Write-Host ""
Write-Host "Applications deployed successfully."
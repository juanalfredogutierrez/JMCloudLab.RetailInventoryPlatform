Write-Host "========================================" -ForegroundColor Cyan
Write-Host " RetailInventory - Starting Platform" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

docker compose up --build -d

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "Failed to start RetailInventory." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Containers:" -ForegroundColor Cyan
docker compose ps

Write-Host ""
Write-Host "RetailInventory started successfully." -ForegroundColor Green
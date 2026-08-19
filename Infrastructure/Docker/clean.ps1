Write-Host "========================================" -ForegroundColor Cyan
Write-Host " RetailInventory - Docker Cleanup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

docker compose down --rmi local --volumes --remove-orphans

docker system prune -f

Write-Host ""
Write-Host "Docker cleanup completed." -ForegroundColor Green
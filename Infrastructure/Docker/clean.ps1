$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host " RetailInventory - Docker Cleanup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

try {

    Write-Host "[1/3] Stopping and removing containers..." -ForegroundColor Yellow

    # IMPORTANTE:
    # No usar --volumes para conservar la data de las BD.
    docker compose down --rmi local --remove-orphans

    if ($LASTEXITCODE -ne 0) {
        throw "Docker Compose cleanup failed."
    }

    Write-Host "      Containers, local images and network removed." -ForegroundColor Green
    Write-Host "      Database volumes preserved." -ForegroundColor Green
    Write-Host ""

    Write-Host "[2/3] Cleaning Docker build cache..." -ForegroundColor Yellow

    docker builder prune -f

    if ($LASTEXITCODE -ne 0) {
        throw "Docker build cache cleanup failed."
    }

    Write-Host "      Build cache cleaned." -ForegroundColor Green
    Write-Host ""

    Write-Host "[3/3] Cleaning unused Docker resources..." -ForegroundColor Yellow

    docker system prune -f

    if ($LASTEXITCODE -ne 0) {
        throw "Docker system cleanup failed."
    }

    Write-Host "      Unused Docker resources cleaned." -ForegroundColor Green
    Write-Host ""

    Write-Host "========================================" -ForegroundColor Green
    Write-Host " Docker cleanup completed." -ForegroundColor Green
    Write-Host " Database data preserved." -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""

}
catch {

    Write-Host ""
    Write-Host "========================================" -ForegroundColor Red
    Write-Host " Docker cleanup failed." -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    Write-Host ""
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""

    exit 1
}
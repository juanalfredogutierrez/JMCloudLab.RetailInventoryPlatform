$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host " RetailInventory - Starting Platform" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

try {

    Write-Host "[1/3] Validating Docker Compose configuration..." -ForegroundColor Yellow

    docker compose config --quiet

    if ($LASTEXITCODE -ne 0) {
        throw "Docker Compose configuration is invalid."
    }

    Write-Host "      Configuration OK." -ForegroundColor Green
    Write-Host ""

    Write-Host "[2/3] Building and starting platform..." -ForegroundColor Yellow

    docker compose up --build -d

    if ($LASTEXITCODE -ne 0) {
        throw "Docker Compose failed to build or start the platform."
    }

    Write-Host ""
    Write-Host "[3/3] Verifying containers..." -ForegroundColor Yellow

    Start-Sleep -Seconds 5

    $services = @(docker compose config --services)

    if ($LASTEXITCODE -ne 0 -or $services.Count -eq 0) {
        throw "Unable to retrieve Docker Compose services."
    }

    $failedServices = @()

    foreach ($service in $services) {

        $containerId = docker compose ps -q $service

        if ([string]::IsNullOrWhiteSpace($containerId)) {
            $failedServices += $service
            continue
        }

        $state = docker inspect `
            --format '{{.State.Status}}' `
            $containerId

        if ($state -ne "running") {
            $failedServices += "$service ($state)"
        }
    }

    Write-Host ""

    if ($failedServices.Count -gt 0) {

        Write-Host "Platform startup failed." -ForegroundColor Red
        Write-Host ""
        Write-Host "Services not running:" -ForegroundColor Red

        foreach ($service in $failedServices) {
            Write-Host " - $service" -ForegroundColor Red
        }

        Write-Host ""
        Write-Host "Current container status:" -ForegroundColor Yellow

        docker compose ps

        exit 1
    }

    Write-Host "All Docker Compose services are running." -ForegroundColor Green
    Write-Host ""

    docker compose ps

    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host " RetailInventory started successfully." -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""

}
catch {

    Write-Host ""
    Write-Host "========================================" -ForegroundColor Red
    Write-Host " RetailInventory startup failed." -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    Write-Host ""
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""

    docker compose ps

    exit 1
}
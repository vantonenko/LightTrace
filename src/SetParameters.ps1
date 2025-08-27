# Run PowerShell as Administrator for system-wide changes

# Set LightTrace environment variables
Write-Host "Setting LightTrace environment variables..." -ForegroundColor Green

# Set report interval (30 seconds)
[Environment]::SetEnvironmentVariable("LIGHT_TRACE_REPORT_INTERVAL", "30", "Machine")
Write-Host "LIGHT_TRACE_REPORT_INTERVAL set to 30 seconds" -ForegroundColor Yellow

# Set report folder
$reportFolder = 'C:\Logs\LightTrace'

[Environment]::SetEnvironmentVariable("LIGHT_TRACE_REPORT_FOLDER", $reportFolder, "Machine")
Write-Host "LIGHT_TRACE_REPORT_FOLDER set to $reportFolder" -ForegroundColor Yellow

# Test each step
Write-Host "Testing path existence..."
$pathExists = Test-Path -Path $reportFolder
Write-Host "Path exists: $pathExists"

if (!$pathExists) {
    Write-Host "Creating directory..."
    New-Item -ItemType Directory -Path $reportFolder -Force
    Write-Host "Directory created."
}

Write-Host "Environment variables set successfully!" -ForegroundColor Green
Write-Host "Note: You may need to restart applications to pick up the new variables." -ForegroundColor Cyan

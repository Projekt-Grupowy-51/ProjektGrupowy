# ============================================
# GitHub Secrets Configuration Script
# ============================================
# This script automatically configures all required GitHub Secrets for VidMark deployment
# Prerequisites: GitHub CLI installed (gh) and authenticated

Write-Host "=== VidMark GitHub Secrets Setup ===" -ForegroundColor Cyan
Write-Host ""

# Check if GitHub CLI is installed
if (!(Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Host "ERROR: GitHub CLI (gh) is not installed!" -ForegroundColor Red
    Write-Host "Install it with: winget install GitHub.cli" -ForegroundColor Yellow
    exit 1
}

# Check if authenticated
$authStatus = gh auth status 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Not authenticated with GitHub CLI!" -ForegroundColor Red
    Write-Host "Run: gh auth login" -ForegroundColor Yellow
    exit 1
}

Write-Host "GitHub CLI detected and authenticated." -ForegroundColor Green
Write-Host ""

# ============================================
# CONFIGURATION - EDIT THESE VALUES
# ============================================

Write-Host "Please provide the following configuration values:" -ForegroundColor Yellow
Write-Host ""

# Database Configuration
Write-Host "=== Database Configuration ===" -ForegroundColor Cyan
$POSTGRES_USER = Read-Host "POSTGRES_USER [default: vidmark_user]"
if ([string]::IsNullOrWhiteSpace($POSTGRES_USER)) { $POSTGRES_USER = "vidmark_user" }

$POSTGRES_PASS = Read-Host "POSTGRES_PASS (leave empty to auto-generate)"
if ([string]::IsNullOrWhiteSpace($POSTGRES_PASS)) {
    $POSTGRES_PASS = -join ((48..57) + (65..90) + (97..122) | Get-Random -Count 32 | ForEach-Object {[char]$_})
    Write-Host "Generated: $POSTGRES_PASS" -ForegroundColor Green
}

$POSTGRES_DB = Read-Host "POSTGRES_DB [default: vidmark]"
if ([string]::IsNullOrWhiteSpace($POSTGRES_DB)) { $POSTGRES_DB = "vidmark" }

Write-Host ""

# Keycloak Configuration
Write-Host "=== Keycloak Configuration ===" -ForegroundColor Cyan
$KEYCLOAK_HOSTNAME = Read-Host "KEYCLOAK_HOSTNAME (e.g., keycloak.example.com)"
while ([string]::IsNullOrWhiteSpace($KEYCLOAK_HOSTNAME)) {
    Write-Host "ERROR: KEYCLOAK_HOSTNAME is required!" -ForegroundColor Red
    $KEYCLOAK_HOSTNAME = Read-Host "KEYCLOAK_HOSTNAME"
}

$KEYCLOAK_ADMIN = Read-Host "KEYCLOAK_ADMIN [default: admin]"
if ([string]::IsNullOrWhiteSpace($KEYCLOAK_ADMIN)) { $KEYCLOAK_ADMIN = "admin" }

$KEYCLOAK_ADMIN_PASSWORD = Read-Host "KEYCLOAK_ADMIN_PASSWORD (leave empty to auto-generate)"
if ([string]::IsNullOrWhiteSpace($KEYCLOAK_ADMIN_PASSWORD)) {
    $KEYCLOAK_ADMIN_PASSWORD = -join ((48..57) + (65..90) + (97..122) | Get-Random -Count 32 | ForEach-Object {[char]$_})
    Write-Host "Generated: $KEYCLOAK_ADMIN_PASSWORD" -ForegroundColor Green
}

$SCIENTIST_ACCESS_CODE = Read-Host "SCIENTIST_ACCESS_CODE (leave empty to auto-generate)"
if ([string]::IsNullOrWhiteSpace($SCIENTIST_ACCESS_CODE)) {
    $SCIENTIST_ACCESS_CODE = "SCIENTIST2024-" + (-join ((48..57) + (65..90) | Get-Random -Count 8 | ForEach-Object {[char]$_}))
    Write-Host "Generated: $SCIENTIST_ACCESS_CODE" -ForegroundColor Green
}

$KEYCLOAK_AUTHORITY = "https://$KEYCLOAK_HOSTNAME/realms/vidmark"
$KEYCLOAK_ISSUER = "https://$KEYCLOAK_HOSTNAME/realms/vidmark"
$KEYCLOAK_AUDIENCE = "vidmark-client"

Write-Host ""

# Frontend URLs
Write-Host "=== Frontend URLs ===" -ForegroundColor Cyan
$API_DOMAIN = Read-Host "API Domain (e.g., api.example.com)"
while ([string]::IsNullOrWhiteSpace($API_DOMAIN)) {
    Write-Host "ERROR: API Domain is required!" -ForegroundColor Red
    $API_DOMAIN = Read-Host "API Domain"
}

$FRONTEND_API_URL = "https://$API_DOMAIN/api"
$FRONTEND_SIGNALR_URL = "https://$API_DOMAIN/hub/app"
$FRONTEND_KEYCLOAK_URL = "https://$KEYCLOAK_HOSTNAME/"

Write-Host "FRONTEND_API_URL: $FRONTEND_API_URL" -ForegroundColor Gray
Write-Host "FRONTEND_SIGNALR_URL: $FRONTEND_SIGNALR_URL" -ForegroundColor Gray
Write-Host "FRONTEND_KEYCLOAK_URL: $FRONTEND_KEYCLOAK_URL" -ForegroundColor Gray

Write-Host ""

# Nginx Configuration
Write-Host "=== Nginx Configuration ===" -ForegroundColor Cyan
$NGINX_DOMAIN = Read-Host "Nginx Domain (e.g., nginx.example.com)"
while ([string]::IsNullOrWhiteSpace($NGINX_DOMAIN)) {
    Write-Host "ERROR: Nginx Domain is required!" -ForegroundColor Red
    $NGINX_DOMAIN = Read-Host "Nginx Domain"
}

$NGINX_HOST = $NGINX_DOMAIN
$NGINX_INTERNAL_URL = "http://nginx/videos"
$NGINX_PUBLIC_BASEURL = "https://$NGINX_DOMAIN"

$NGINX_SECURELINK_SECRET = Read-Host "NGINX_SECURELINK_SECRET (leave empty to auto-generate)"
if ([string]::IsNullOrWhiteSpace($NGINX_SECURELINK_SECRET)) {
    $NGINX_SECURELINK_SECRET = -join ((48..57) + (65..90) + (97..122) | Get-Random -Count 32 | ForEach-Object {[char]$_})
    Write-Host "Generated: $NGINX_SECURELINK_SECRET" -ForegroundColor Green
}

Write-Host ""

# CORS Configuration
Write-Host "=== CORS Configuration ===" -ForegroundColor Cyan
$FRONTEND_DOMAIN = Read-Host "Frontend Domain (e.g., example.com or www.example.com)"
while ([string]::IsNullOrWhiteSpace($FRONTEND_DOMAIN)) {
    Write-Host "ERROR: Frontend Domain is required!" -ForegroundColor Red
    $FRONTEND_DOMAIN = Read-Host "Frontend Domain"
}

$CORS_ALLOWED_ORIGIN = "https://$FRONTEND_DOMAIN"

Write-Host ""

# Grafana Configuration
Write-Host "=== Grafana Configuration ===" -ForegroundColor Cyan
$GRAFANA_ADMIN_USER = Read-Host "GRAFANA_ADMIN_USER [default: admin]"
if ([string]::IsNullOrWhiteSpace($GRAFANA_ADMIN_USER)) { $GRAFANA_ADMIN_USER = "admin" }

$GRAFANA_ADMIN_PASSWORD = Read-Host "GRAFANA_ADMIN_PASSWORD (leave empty to auto-generate)"
if ([string]::IsNullOrWhiteSpace($GRAFANA_ADMIN_PASSWORD)) {
    $GRAFANA_ADMIN_PASSWORD = -join ((48..57) + (65..90) + (97..122) | Get-Random -Count 32 | ForEach-Object {[char]$_})
    Write-Host "Generated: $GRAFANA_ADMIN_PASSWORD" -ForegroundColor Green
}

Write-Host ""

# VPS Access
Write-Host "=== VPS Access ===" -ForegroundColor Cyan
$VPS_HOST = Read-Host "VPS_HOST (IP address)"
while ([string]::IsNullOrWhiteSpace($VPS_HOST)) {
    Write-Host "ERROR: VPS_HOST is required!" -ForegroundColor Red
    $VPS_HOST = Read-Host "VPS_HOST"
}

$VPS_USER = Read-Host "VPS_USER [default: vidmark]"
if ([string]::IsNullOrWhiteSpace($VPS_USER)) { $VPS_USER = "vidmark" }

$VPS_PASSWORD = Read-Host "VPS_PASSWORD" -AsSecureString
$VPS_PASSWORD_PLAIN = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($VPS_PASSWORD))

Write-Host ""

# GitHub Container Registry
Write-Host "=== GitHub Container Registry ===" -ForegroundColor Cyan
Write-Host "You need a Personal Access Token with 'write:packages' and 'read:packages' permissions." -ForegroundColor Yellow
Write-Host "Create one at: https://github.com/settings/tokens/new" -ForegroundColor Yellow
$GHCR_TOKEN = Read-Host "GHCR_TOKEN (starts with ghp_)"
while ([string]::IsNullOrWhiteSpace($GHCR_TOKEN)) {
    Write-Host "ERROR: GHCR_TOKEN is required!" -ForegroundColor Red
    $GHCR_TOKEN = Read-Host "GHCR_TOKEN"
}

Write-Host ""
Write-Host "=== Configuration Summary ===" -ForegroundColor Cyan
Write-Host "Database: $POSTGRES_USER @ $POSTGRES_DB" -ForegroundColor Gray
Write-Host "Keycloak: https://$KEYCLOAK_HOSTNAME" -ForegroundColor Gray
Write-Host "API: https://$API_DOMAIN" -ForegroundColor Gray
Write-Host "Nginx: https://$NGINX_DOMAIN" -ForegroundColor Gray
Write-Host "Frontend: https://$FRONTEND_DOMAIN" -ForegroundColor Gray
Write-Host "VPS: $VPS_USER@$VPS_HOST" -ForegroundColor Gray
Write-Host ""

$confirm = Read-Host "Do you want to proceed with setting these secrets? (yes/no)"
if ($confirm -ne "yes") {
    Write-Host "Setup cancelled." -ForegroundColor Yellow
    exit 0
}

Write-Host ""
Write-Host "=== Setting GitHub Secrets ===" -ForegroundColor Cyan

# Function to set secret
function Set-GitHubSecret {
    param (
        [string]$Name,
        [string]$Value
    )

    Write-Host "Setting $Name..." -NoNewline
    $result = echo $Value | gh secret set $Name 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host " OK" -ForegroundColor Green
        return $true
    } else {
        Write-Host " FAILED" -ForegroundColor Red
        Write-Host "Error: $result" -ForegroundColor Red
        return $false
    }
}

# Set all secrets
$success = $true

# Database
$success = $success -and (Set-GitHubSecret "POSTGRES_USER" $POSTGRES_USER)
$success = $success -and (Set-GitHubSecret "POSTGRES_PASS" $POSTGRES_PASS)
$success = $success -and (Set-GitHubSecret "POSTGRES_DB" $POSTGRES_DB)

# Keycloak
$success = $success -and (Set-GitHubSecret "KEYCLOAK_HOSTNAME" $KEYCLOAK_HOSTNAME)
$success = $success -and (Set-GitHubSecret "KEYCLOAK_ADMIN" $KEYCLOAK_ADMIN)
$success = $success -and (Set-GitHubSecret "KEYCLOAK_ADMIN_PASSWORD" $KEYCLOAK_ADMIN_PASSWORD)
$success = $success -and (Set-GitHubSecret "SCIENTIST_ACCESS_CODE" $SCIENTIST_ACCESS_CODE)
$success = $success -and (Set-GitHubSecret "KEYCLOAK_AUTHORITY" $KEYCLOAK_AUTHORITY)
$success = $success -and (Set-GitHubSecret "KEYCLOAK_ISSUER" $KEYCLOAK_ISSUER)
$success = $success -and (Set-GitHubSecret "KEYCLOAK_AUDIENCE" $KEYCLOAK_AUDIENCE)

# Frontend URLs
$success = $success -and (Set-GitHubSecret "FRONTEND_API_URL" $FRONTEND_API_URL)
$success = $success -and (Set-GitHubSecret "FRONTEND_SIGNALR_URL" $FRONTEND_SIGNALR_URL)
$success = $success -and (Set-GitHubSecret "FRONTEND_KEYCLOAK_URL" $FRONTEND_KEYCLOAK_URL)

# Nginx
$success = $success -and (Set-GitHubSecret "NGINX_HOST" $NGINX_HOST)
$success = $success -and (Set-GitHubSecret "NGINX_INTERNAL_URL" $NGINX_INTERNAL_URL)
$success = $success -and (Set-GitHubSecret "NGINX_PUBLIC_BASEURL" $NGINX_PUBLIC_BASEURL)
$success = $success -and (Set-GitHubSecret "NGINX_SECURELINK_SECRET" $NGINX_SECURELINK_SECRET)

# CORS
$success = $success -and (Set-GitHubSecret "CORS_ALLOWED_ORIGIN" $CORS_ALLOWED_ORIGIN)

# Grafana
$success = $success -and (Set-GitHubSecret "GRAFANA_ADMIN_USER" $GRAFANA_ADMIN_USER)
$success = $success -and (Set-GitHubSecret "GRAFANA_ADMIN_PASSWORD" $GRAFANA_ADMIN_PASSWORD)

# VPS
$success = $success -and (Set-GitHubSecret "VPS_HOST" $VPS_HOST)
$success = $success -and (Set-GitHubSecret "VPS_USER" $VPS_USER)
$success = $success -and (Set-GitHubSecret "VPS_PASSWORD" $VPS_PASSWORD_PLAIN)

# GHCR
$success = $success -and (Set-GitHubSecret "GHCR_TOKEN" $GHCR_TOKEN)

Write-Host ""

if ($success) {
    Write-Host "=== SUCCESS ===" -ForegroundColor Green
    Write-Host "All secrets have been configured successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Cyan
    Write-Host "1. Go to GitHub Actions and run 'Build Container Images' workflow" -ForegroundColor White
    Write-Host "2. After build completes, run 'Deploy to Production' workflow" -ForegroundColor White
    Write-Host ""
    Write-Host "Verify secrets with: gh secret list" -ForegroundColor Gray
} else {
    Write-Host "=== ERRORS OCCURRED ===" -ForegroundColor Red
    Write-Host "Some secrets failed to set. Please check the errors above." -ForegroundColor Red
    Write-Host "You can manually set missing secrets at:" -ForegroundColor Yellow
    Write-Host "https://github.com/Projekt-Grupowy-51/ProjektGrupowy/settings/secrets/actions" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "IMPORTANT: Save these generated passwords in a secure location!" -ForegroundColor Yellow
Write-Host ""
Write-Host "Generated credentials:" -ForegroundColor Cyan
Write-Host "POSTGRES_PASS: $POSTGRES_PASS" -ForegroundColor Gray
Write-Host "KEYCLOAK_ADMIN_PASSWORD: $KEYCLOAK_ADMIN_PASSWORD" -ForegroundColor Gray
Write-Host "SCIENTIST_ACCESS_CODE: $SCIENTIST_ACCESS_CODE" -ForegroundColor Gray
Write-Host "NGINX_SECURELINK_SECRET: $NGINX_SECURELINK_SECRET" -ForegroundColor Gray
Write-Host "GRAFANA_ADMIN_PASSWORD: $GRAFANA_ADMIN_PASSWORD" -ForegroundColor Gray
Write-Host ""

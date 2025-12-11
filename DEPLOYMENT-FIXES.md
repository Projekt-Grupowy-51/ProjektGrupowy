# Deployment Fixes Summary

## Problems Fixed

### 1. ✅ Nginx Video Serving (403 Error)
**Files:** `nginx/nginx.conf.template`, `nginx/entrypoint.sh`

**Problem:** Hardcoded secret `"super-secret-link-key"` instead of environment variable

**Solution:**
- Changed `secure_link_md5` to use `${NGINX_SECURELINK_SECRET}` variable
- Added `${NGINX_SECURELINK_SECRET}` to `envsubst` in entrypoint.sh

### 2. ✅ Keycloak Admin Panel Access
**File:** `nginx-gateway/nginx.conf.template`

**Problem:** Gateway was sending `X-Forwarded-Proto: http` instead of `https`, causing Keycloak to generate incorrect redirects

**Solution:**
- Added logic to detect and set `X-Forwarded-Proto` to `https`
- Updated all locations in gateway to use `$forwarded_proto` variable
- Keycloak now correctly recognizes that it's behind HTTPS

### 3. ✅ Keycloak Theme Loading
**File:** `keycloak/Dockerfile`

**Problem:** `themes/videomark` directory was not being copied to Docker image

**Solution:**
- Added `COPY themes/videomark /opt/keycloak/themes/videomark` to Dockerfile
- Themes are now available in Keycloak

### 4. ✅ Grafana Access
**Files:** `nginx-gateway/nginx.conf.template`, `docker-compose.prod.yml`

**Problem:** No access to Grafana through gateway

**Solution:**
- Added `grafana` upstream in nginx-gateway
- Added `/grafana/` location with full proxy pass
- Added WebSocket support (Grafana Live)
- Configured Grafana to work under subpath:
  - `GF_SERVER_ROOT_URL: https://${DOMAIN}/grafana/`
  - `GF_SERVER_SERVE_FROM_SUB_PATH: "true"`

### 5. ✅ Security: Removed Exposed Ports
**File:** `docker-compose.prod.yml`

**Problem:** Internal services had ports exposed to host, allowing bypass of gateway

**Solution:**
- Removed port `18133:8080` from `keycloak`
- Removed port `18130:80` from `app` (backend)
- Removed port `18131:80` from `frontend`
- Only `gateway` port `80:80` remains exposed (single entry point)

### 6. ✅ Fixed GitHub Secrets Script
**File:** `setup-github-secrets.ps1`

**Problem:** Script used old subdomain architecture instead of single domain with gateway

**Solution:**
- Removed subdomain prompts (`KEYCLOAK_HOSTNAME`, `API_DOMAIN`, `NGINX_DOMAIN`, etc.)
- Simplified to single `DOMAIN` variable
- Fixed hardcoded `NGINX_SECURELINK_SECRET` - now auto-generates 64-char secret
- Removed unused secrets:
  - `KEYCLOAK_HOSTNAME`, `KEYCLOAK_AUTHORITY`, `KEYCLOAK_ISSUER`, `KEYCLOAK_AUDIENCE`
  - `FRONTEND_API_URL`, `FRONTEND_SIGNALR_URL`, `FRONTEND_KEYCLOAK_URL`
  - `NGINX_HOST`, `NGINX_INTERNAL_URL`, `NGINX_PUBLIC_BASEURL`
  - `CORS_ALLOWED_ORIGIN`

### 7. ✅ Cleaned Up deploy.yml
**File:** `.github/workflows/deploy.yml`

**Problem:** Referenced unused GitHub Secrets

**Solution:**
- Removed all unused secret references from `.env` generation
- Only keeps secrets that are actually used in `docker-compose.prod.yml`

## Architecture After Fixes

```
Internet (HTTPS)
    ↓
Cloudflare/LB (SSL termination)
    ↓
VPS:80 → gateway:80 (SINGLE PUBLIC ENTRY POINT) ✅
    ↓
├─→ frontend:80 (internal Docker network) 🔒
├─→ app:80 (internal Docker network) 🔒
├─→ keycloak:8080 (internal Docker network) 🔒
├─→ nginx-videos:80 (internal Docker network) 🔒
└─→ grafana:3000 (internal Docker network) 🔒
```

## Required GitHub Secrets

Run `./setup-github-secrets.ps1` to configure these:

### Core Secrets
- `DOMAIN` - Main domain (e.g., vidmistrz.pl)
- `POSTGRES_USER` - Database username
- `POSTGRES_PASS` - Database password (auto-generated)
- `POSTGRES_DB` - Database name
- `KEYCLOAK_ADMIN` - Keycloak admin username
- `KEYCLOAK_ADMIN_PASSWORD` - Keycloak admin password (auto-generated)
- `SCIENTIST_ACCESS_CODE` - Access code for scientists (auto-generated)
- `NGINX_SECURELINK_SECRET` - Secret for secure video links (auto-generated 64-char)
- `GRAFANA_ADMIN_USER` - Grafana admin username
- `GRAFANA_ADMIN_PASSWORD` - Grafana admin password (auto-generated)
- `VPS_HOST` - VPS IP address
- `VPS_USER` - VPS SSH username
- `VPS_PASSWORD` - VPS SSH password

## Service URLs

All services accessible through single domain:

- **Frontend**: `https://vidmistrz.pl/`
- **Backend API**: `https://vidmistrz.pl/api`
- **Keycloak**: `https://vidmistrz.pl/auth`
- **Keycloak Admin**: `https://vidmistrz.pl/auth/admin`
- **Videos**: `https://vidmistrz.pl/videos`
- **Grafana**: `https://vidmistrz.pl/grafana` (when monitoring enabled)
- **Swagger**: `https://vidmistrz.pl/swagger` (optional, for debugging)

## Deployment Steps

1. **Setup GitHub Secrets** (one-time):
   ```powershell
   .\setup-github-secrets.ps1
   ```

2. **Build Docker Images**:
   ```bash
   # Build all images
   gh workflow run build-images.yml -f images=all -f tag=v1.0.0

   # Or build only specific images
   gh workflow run build-images.yml -f images=nginx,gateway,keycloak -f tag=v1.0.0
   ```

3. **Deploy to Production**:
   ```bash
   gh workflow run deploy.yml \
     -f backend_tag=v1.0.0 \
     -f frontend_tag=v1.0.0 \
     -f keycloak_tag=v1.0.0 \
     -f nginx_tag=v1.0.0 \
     -f gateway_tag=v1.0.0 \
     -f enable_monitoring=true
   ```

## Security Notes

### ✅ What's Secure:
- External traffic uses HTTPS (SSL terminated at Cloudflare/LB)
- Only gateway exposed to internet (port 80)
- All internal services isolated in Docker network
- No way to bypass gateway from external network
- Secure video links with secret-based authentication

### ⚠️ Internal Traffic:
- Services communicate via HTTP inside Docker (unencrypted)
- This is OK because:
  - Docker network is isolated
  - Traffic never leaves the host
  - Minimizes encryption overhead
- If host is compromised, traffic can be sniffed

### 🔒 Additional Security (Optional):
- PostgreSQL SSL connections
- Rate limiting in nginx-gateway
- Fail2ban on VPS host
- Docker network encryption (requires Docker Swarm)

## Monitoring

When `enable_monitoring=true`:
- Prometheus metrics: Direct port `9090` or via custom route
- Loki logs: Direct port `3100` or via custom route
- Grafana dashboards: `https://vidmistrz.pl/grafana/`

## Debugging

To enable direct access to services for debugging, uncomment these lines in `docker-compose.prod.yml`:

```yaml
# keycloak:
#   ports: ["18133:8080"]

# app:
#   ports: ["18130:80"]

# frontend:
#   ports: ["18131:80"]
```

**Remember to remove these in production!**

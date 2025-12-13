# VideoMark Keycloak Configuration

Unified Keycloak directory containing both themes and realm data.

## Structure

```
keycloak/
├── data/
│   └── import/
│       └── vidmark-realm.json   # Realm configuration for auto-import
└── themes/
    └── videomark/
        ├── theme.properties     # Main theme configuration
        └── login/
            ├── theme.properties # Login theme configuration
            ├── login.ftl       # Custom login template
            └── resources/
                └── css/
                    └── videomark.css # VideoMark custom styling
```

## Docker Setup

Keycloak data and themes are mounted bidirectionally:

```yaml
volumes:
  - ./keycloak/data:/opt/keycloak/data      # Bidirectional data persistence
  - ./keycloak/themes:/opt/keycloak/themes  # Custom themes
```

This provides:
- **Bidirectional sync**: Changes in Admin Console are saved to host
- **Themes**: Available at `/opt/keycloak/themes/videomark`
- **Realm Import**: Available at `/opt/keycloak/data/import/vidmark-realm.json`
- **Data persistence**: Database, cache, logs, exports saved to host

## Usage

1. **Start Keycloak**:
   ```bash
   docker-compose up keycloak
   ```

2. **Configure Theme**:
   - Go to Admin Console: http://localhost:8080
   - Login: admin/admin
   - Realm Settings → Themes → Login Theme: `videomark`

3. **View Custom Login**:
   - Visit: http://localhost:8080/realms/vidmark/account
   - See VideoMark branded login form

## Bidirectional Data Sync

With the current setup, all changes are automatically synchronized:

### ✅ **Host → Keycloak**:
- Theme changes in `./keycloak/themes/` → instantly visible
- Realm import from `./keycloak/data/import/` on startup

### ✅ **Keycloak → Host**:  
- Admin Console changes → saved to `./keycloak/data/`
- New users, roles, settings → persisted on host
- Logs → available in `./keycloak/data/log/`
- Exported realms → saved to `./keycloak/data/export/`

### 📂 **Data Structure**:
```
keycloak/data/
├── import/       # Realm imports (startup)
├── export/       # Realm exports  
├── cache/        # Keycloak cache
├── log/          # Application logs
└── tmp/          # Temporary files
```

## Theme Features

- 🎬 VideoMark branding with movie icon
- 🎨 Modern gradient background (blue → dark blue)  
- 📱 Responsive design for mobile/desktop
- ✨ Glassmorphism effects and smooth animations
- 🔧 Customizable colors via CSS variables

## Export Realm Configuration

To backup current realm settings:

```bash
# Export to host directory  
docker exec keycloak /opt/keycloak/bin/kc.sh export \
  --dir /opt/keycloak/data/export \
  --realm vidmark \
  --users realm_file

# Files will appear in ./keycloak/data/export/
```
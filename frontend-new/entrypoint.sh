#!/bin/sh

# Generate runtime configuration file that will be loaded by the app
cat > /usr/share/nginx/html/env-config.js <<EOF
// Runtime environment configuration
window.ENV = {
  VITE_API_BASE_URL: "${VITE_API_BASE_URL}",
  VITE_SIGNALR_HUB_URL: "${VITE_SIGNALR_HUB_URL}",
  VITE_KEYCLOAK_URL: "${VITE_KEYCLOAK_URL}",
  VITE_KEYCLOAK_REALM: "${VITE_KEYCLOAK_REALM}",
  VITE_KEYCLOAK_CLIENT_ID: "${VITE_KEYCLOAK_CLIENT_ID}"
};
EOF

echo "Runtime environment configuration generated:"
cat /usr/share/nginx/html/env-config.js

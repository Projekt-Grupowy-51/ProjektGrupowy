import Keycloak from 'keycloak-js';

// Konfiguracja z runtime ENV (produkcja) lub build-time (dev) lub wartości domyślnych
const keycloakConfig = {
  url: window.ENV?.VITE_KEYCLOAK_URL || import.meta.env.VITE_KEYCLOAK_URL || 'http://localhost:8080/',
  realm: window.ENV?.VITE_KEYCLOAK_REALM || import.meta.env.VITE_KEYCLOAK_REALM || 'vidmark',
  clientId: window.ENV?.VITE_KEYCLOAK_CLIENT_ID || import.meta.env.VITE_KEYCLOAK_CLIENT_ID || 'vidmark-client',
};

// Singleton pattern - tworzy tylko jedną instancję Keycloak
let keycloakInstance = null;

const getKeycloakInstance = () => {
  if (!keycloakInstance) {
    keycloakInstance = new Keycloak(keycloakConfig);
  }
  return keycloakInstance;
};

const keycloak = getKeycloakInstance();

export default keycloak;
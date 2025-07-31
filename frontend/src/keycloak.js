import Keycloak from 'keycloak-js';

// Konfiguracja z zmiennych środowiskowych lub wartości domyślnych
const keycloakConfig = {
  url: import.meta.env.VITE_KEYCLOAK_URL || 'http://localhost:8080/',
  realm: import.meta.env.VITE_KEYCLOAK_REALM || 'vidmark',
  clientId: import.meta.env.VITE_KEYCLOAK_CLIENT_ID || 'vidmark-client',
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

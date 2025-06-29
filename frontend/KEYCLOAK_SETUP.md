# Konfiguracja Keycloak

## Konfiguracja Realm

1. Zaloguj się do Admin Console
2. Stwórz nowy realm o nazwie: `projektgrupowy`
3. W zakładce "Clients" stwórz nowego klienta:
   - Client ID: `projektgrupowy-client`
   - Client authentication: OFF (public client)
   - Valid redirect URIs: `http://localhost:3000/*`, `http://localhost:3000` (dodaj te dwa)
   - Valid postredirect URIs jak wyżej
   - Web origins: `http://localhost:3000`

## Konfiguracja Ról

1. W zakładce "Realm roles" stwórz role:
   - `Admin`
   - `Scientist` 
   - `Labeler`

2. W zakładce "Clients" -> `projektgrupowy-client` -> "Roles" stwórz role klienta:
   - `Admin`
   - `Scientist`
   - `Labeler`

3. Usuń wszystkie domyślne role i dodaj rolę `Labeler` (Realm settings -> User Registration)

## Zmienne środowiskowe

Utwórz plik `.env` w folderze `frontend` z następującymi zmiennymi:

```env
VITE_KEYCLOAK_URL=http://localhost:8080/
VITE_KEYCLOAK_REALM=projektgrupowy
VITE_KEYCLOAK_CLIENT_ID=projektgrupowy-client
```

- Aplikacja używa `check-sso` mode w KeycloakProvider, co pozwala na silent login
- W przypadku 401 z backendu, automatycznie próbuje odnowić token
- Jeśli odświeżenie nie powiedzie się, przekierowuje do Keycloak login
- Role są pobierane zarówno z realm_access jak i resource_access dla klienta
- Frontend działa na porcie 3000 (nie 5173 jak Vite)
- Uproszczona architektura - tylko KeycloakProvider, usunięto AuthContext i auth.js

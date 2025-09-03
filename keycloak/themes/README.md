# VideoMark Keycloak Custom Theme

## Instalacja i konfiguracja

### 1. Uruchom Keycloak z custom theme
```bash
docker-compose up keycloak
```

### 2. Skonfiguruj theme w Keycloak Admin Console

1. Wejdź do Keycloak Admin Console: http://localhost:8080
2. Zaloguj się jako admin (login: admin, password: admin)
3. Wybierz realm "vidmark" (lub stwórz nowy)
4. Idź do **Realm Settings** → **Themes**
5. Ustaw:
   - **Login Theme**: `videomark`
   - **Account Theme**: `videomark` (opcjonalnie)
6. Kliknij **Save**

### 3. Testuj custom theme
1. Wejdź na stronę logowania: http://localhost:8080/realms/vidmark/account
2. Powinieneś zobaczyć custom VideoMark theme z:
   - Gradient background (niebieski → granatowy)
   - VideoMark logo z ikoną 🎬
   - Nowoczesne, zaokrąglone formularze
   - Custom kolory i styling

## Struktura theme

```
keycloak-themes/
└── videomark/
    ├── theme.properties          # Główna konfiguracja theme
    └── login/
        ├── theme.properties      # Konfiguracja login theme
        ├── login.ftl            # Custom template logowania
        └── resources/
            └── css/
                └── videomark.css # Custom styling
```

## Dostosowywanie

### Kolory
Edytuj zmienne CSS w `videomark.css`:
```css
:root {
  --videomark-primary: #3498db;    /* Główny kolor niebieski */
  --videomark-secondary: #2c3e50;  /* Ciemny niebieski */
  --videomark-accent: #e74c3c;     /* Czerwony akcent */
}
```

### Logo/Branding
W pliku `login.ftl` znajdź:
```html
<div class="kc-logo-text">VideoMark</div>
<p style="color: #7f8c8d; margin-top: 0.5rem;">Video Analysis & Labeling Platform</p>
```

### Dodawanie nowych stron
1. Skopiuj template z base theme
2. Dostosuj HTML w .ftl files  
3. Dodaj custom CSS w `videomark.css`

## Troubleshooting

- **Theme nie widać**: Sprawdź czy volume w docker-compose jest poprawnie zamontowany
- **Zmiany CSS nie działają**: Wyczyść cache przeglądarki lub użyj trybu incognito
- **500 error**: Sprawdź logi Keycloak: `docker-compose logs keycloak`
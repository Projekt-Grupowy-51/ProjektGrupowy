# ==========================
# Etap budowy
FROM node:18 AS builder

ARG VITE_API_BASE_URL
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL

ARG VITE_SIGNALR_HUB_URL
ENV VITE_SIGNALR_HUB_URL=$VITE_SIGNALR_HUB_URL

# Ustawienie katalogu roboczego
WORKDIR /app

# Kopiowanie plików package.json i package-lock.json
COPY package.json package-lock.json ./

# Instalowanie zależności
RUN npm install

# Kopiowanie całego projektu
COPY . .

# Budowanie aplikacji
RUN npm run build

# ==========================
# Etap serwera produkcyjnego
FROM nginx:alpine

# Usunięcie domyślnych plików Nginx
RUN rm -rf /usr/share/nginx/html/*

# Kopiowanie zbudowanej aplikacji z pierwszego etapu
COPY --from=builder /app/dist /usr/share/nginx/html

# Kopiowanie własnej konfiguracji Nginx
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Exponowanie portu 80
EXPOSE 80

# Uruchamianie Nginx
CMD ["nginx", "-g", "daemon off;"]

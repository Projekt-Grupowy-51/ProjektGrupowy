# Etap budowy
FROM node:18 AS builder

# Ustawienie katalogu roboczego
WORKDIR /app

# Kopiowanie plików package.json i package-lock.json
COPY package.json package-lock.json ./

# Instalowanie zale¿noœci
RUN npm install

# Kopiowanie ca³ego projektu
COPY . .

# Budowanie aplikacji
RUN npm run build

# ==========================
# Etap serwera produkcyjnego
FROM nginx:alpine

# Usuniêcie domyœlnych plików Nginx
RUN rm -rf /usr/share/nginx/html/*

# Kopiowanie zbudowanej aplikacji z pierwszego etapu
COPY --from=builder /app/dist /usr/share/nginx/html

# Exponowanie portu 80
EXPOSE 80

# Uruchamianie Nginx
CMD ["nginx", "-g", "daemon off;"]

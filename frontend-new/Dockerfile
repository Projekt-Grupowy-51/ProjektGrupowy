# Stage 1: Build React application
FROM node:20-alpine AS builder

WORKDIR /app

# Copy package files
COPY package.json package-lock.json ./

# Install dependencies
RUN npm ci --prefer-offline --no-audit

# Copy source code
COPY . .

# Build production bundle (bez zmiennych - będą w runtime)
RUN npm run build

# Stage 2: Serve with Nginx
FROM nginx:1.25-alpine

# Remove default nginx config
RUN rm /etc/nginx/conf.d/default.conf

# Copy custom nginx configuration
COPY nginx.conf /etc/nginx/nginx.conf

# Copy built application
COPY --from=builder /app/dist /usr/share/nginx/html

# Copy runtime configuration script
COPY entrypoint.sh /docker-entrypoint.d/40-envsubst-on-templates.sh
RUN chmod +x /docker-entrypoint.d/40-envsubst-on-templates.sh

# Environment variables for runtime injection
ENV VITE_API_BASE_URL=""
ENV VITE_SIGNALR_HUB_URL=""
ENV VITE_KEYCLOAK_URL=""
ENV VITE_KEYCLOAK_REALM="vidmark"
ENV VITE_KEYCLOAK_CLIENT_ID="vidmark-client"

EXPOSE 80

# Nginx will run entrypoint scripts from /docker-entrypoint.d/ automatically
CMD ["nginx", "-g", "daemon off;"]

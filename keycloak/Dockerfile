FROM maven:3.9.4-eclipse-temurin-17 AS builder

WORKDIR /app

COPY keycloak-custom-spi/pom.xml .
COPY keycloak-custom-spi/src ./src

RUN mvn clean package -DskipTests

FROM quay.io/keycloak/keycloak:24.0

RUN mkdir -p /opt/keycloak/providers

COPY --from=builder /app/target/*.jar /opt/keycloak/providers/

# Ensure data folder exists for realm import
RUN mkdir -p /opt/keycloak/data/import

# Copy realm configuration for auto-import
COPY data/import/vidmark-realm.json /opt/keycloak/data/import/

# Ensure themes folder exists
RUN mkdir -p /opt/keycloak/themes

# Build optimized Keycloak for production
RUN /opt/keycloak/bin/kc.sh build

# Default command (will be overridden in docker-compose)
CMD ["start", "--optimized", "--import-realm"]

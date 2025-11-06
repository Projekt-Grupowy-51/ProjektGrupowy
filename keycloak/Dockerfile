FROM maven:3.9.4-eclipse-temurin-17 AS builder

WORKDIR /app

COPY keycloak-custom-spi/pom.xml .
COPY keycloak-custom-spi/src ./src

RUN mvn clean package -DskipTests

FROM quay.io/keycloak/keycloak:24.0

RUN mkdir -p /opt/keycloak/providers

COPY --from=builder /app/target/*.jar /opt/keycloak/providers/

# Ensure data and themes folders exist
RUN mkdir -p /opt/keycloak/data /opt/keycloak/themes

# Default command (dev mode)
CMD ["start-dev", "--import-realm"]

# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build image for compiling
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ProjektGrupowy.API.csproj", "ProjektGrupowy.API/"]
RUN dotnet restore "ProjektGrupowy.API/ProjektGrupowy.API.csproj"

WORKDIR "/src/ProjektGrupowy.API"
COPY . .

# Build the application
RUN dotnet build "ProjektGrupowy.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ProjektGrupowy.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image for runtime
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN apt update && apt install -y curl
ENTRYPOINT ["dotnet", "ProjektGrupowy.API.dll"]
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY Directory.Packages.props ./
COPY ProjektGrupowy.API/ProjektGrupowy.API.csproj ProjektGrupowy.API/
COPY ProjektGrupowy.Application/ProjektGrupowy.Application.csproj ProjektGrupowy.Application/
COPY ProjektGrupowy.Domain/ProjektGrupowy.Domain.csproj ProjektGrupowy.Domain/
COPY ProjektGrupowy.Infrastructure/ProjektGrupowy.Infrastructure.csproj ProjektGrupowy.Infrastructure/

RUN dotnet restore ProjektGrupowy.API/ProjektGrupowy.API.csproj

COPY . .

WORKDIR /src/ProjektGrupowy.API
RUN dotnet publish ProjektGrupowy.API.csproj \
    -c ${BUILD_CONFIGURATION} \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

USER root
RUN apt-get update \
 && apt-get install -y --no-install-recommends \
      ffmpeg \
      curl \
      ca-certificates \
 && rm -rf /var/lib/apt/lists/*

RUN mkdir -p /app/videos /app/reports /app/Logs /home/app/.aspnet/DataProtection-Keys \
 && chown -R app:app /app /home/app

COPY --from=build /app/publish/ ./

ENV ASPNETCORE_URLS=http://+:80 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

EXPOSE 80

USER app
ENTRYPOINT ["dotnet", "ProjektGrupowy.API.dll"]

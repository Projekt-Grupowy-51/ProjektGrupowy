FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY Directory.Packages.props ./
COPY VidMark.API/VidMark.API.csproj VidMark.API/
COPY VidMark.Application/VidMark.Application.csproj VidMark.Application/
COPY VidMark.Domain/VidMark.Domain.csproj VidMark.Domain/
COPY VidMark.Infrastructure/VidMark.Infrastructure.csproj VidMark.Infrastructure/

RUN dotnet restore VidMark.API/VidMark.API.csproj

COPY . .

WORKDIR /src/VidMark.API
RUN dotnet publish VidMark.API.csproj \
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
ENTRYPOINT ["dotnet", "VidMark.API.dll"]

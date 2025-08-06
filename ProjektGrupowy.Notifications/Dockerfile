FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ProjektGrupowy.Notifications/ProjektGrupowy.Notifications.csproj", "ProjektGrupowy.Notifications/"]
COPY ["ProjektGrupowy.Application/ProjektGrupowy.Application.csproj", "ProjektGrupowy.Application/"]
COPY ["ProjektGrupowy.Domain/ProjektGrupowy.Domain.csproj", "ProjektGrupowy.Domain/"]
COPY ["ProjektGrupowy.Infrastructure/ProjektGrupowy.Infrastructure.csproj", "ProjektGrupowy.Infrastructure/"]
RUN dotnet restore "ProjektGrupowy.Notifications/ProjektGrupowy.Notifications.csproj"
COPY . .
WORKDIR "/src/ProjektGrupowy.Notifications"
RUN dotnet build "./ProjektGrupowy.Notifications.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ProjektGrupowy.Notifications.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProjektGrupowy.Notifications.dll"]

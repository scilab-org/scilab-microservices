FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/Lab/Worker/Lab.Worker.Outbox/Lab.Worker.Outbox.csproj", "src/Services/Lab/Worker/Lab.Worker.Outbox/"]
COPY ["src/Shared/Common/Common.csproj", "src/Shared/Common/"]
COPY ["src/Shared/EventSourcing/EventSourcing.csproj", "src/Shared/EventSourcing/"]
COPY ["src/Services/Lab/Core/Lab.Application/Lab.Application.csproj", "src/Services/Lab/Core/Lab.Application/"]
COPY ["src/Shared/BuildingBlocks/BuildingBlocks.csproj", "src/Shared/BuildingBlocks/"]
COPY ["src/Services/Lab/Core/Lab.Domain/Lab.Domain.csproj", "src/Services/Lab/Core/Lab.Domain/"]
COPY ["src/Services/Lab/Core/Lab.Infrastructure/Lab.Infrastructure.csproj", "src/Services/Lab/Core/Lab.Infrastructure/"]
COPY ["src/Directory.Build.props", "src/"]
COPY ["src/Directory.Packages.props", "src/"]
RUN dotnet restore "src/Services/Lab/Worker/Lab.Worker.Outbox/Lab.Worker.Outbox.csproj"
COPY . .
WORKDIR "/src/src/Services/Lab/Worker/Lab.Worker.Outbox"
RUN dotnet build "./Lab.Worker.Outbox.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Lab.Worker.Outbox.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lab.Worker.Outbox.dll"]

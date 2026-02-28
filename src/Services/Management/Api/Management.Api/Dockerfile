FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/Management/Api/Management.Api/Management.Api.csproj", "src/Services/Management/Api/Management.Api/"]
COPY ["src/Services/Management/Core/Management.Application/Management.Application.csproj", "src/Services/Management/Core/Management.Application/"]
COPY ["src/Shared/BuildingBlocks/BuildingBlocks.csproj", "src/Shared/BuildingBlocks/"]
COPY ["src/Shared/Common/Common.csproj", "src/Shared/Common/"]
COPY ["src/Shared/EventSourcing/EventSourcing.csproj", "src/Shared/EventSourcing/"]
COPY ["src/Services/Management/Core/Management.Domain/Management.Domain.csproj", "src/Services/Management/Core/Management.Domain/"]
COPY ["src/Services/Management/Core/Management.Infrastructure/Management.Infrastructure.csproj", "src/Services/Management/Core/Management.Infrastructure/"]
COPY ["src/Directory.Build.props", "src/"]
COPY ["src/Directory.Packages.props", "src/"]
RUN dotnet restore "src/Services/Management/Api/Management.Api/Management.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/Management/Api/Management.Api"
RUN dotnet build "./Management.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Management.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Management.Api.dll"]

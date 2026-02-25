FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/User/Api/User.Api/User.Api.csproj", "src/Services/User/Api/User.Api/"]
COPY ["src/Services/User/Core/User.Application/User.Application.csproj", "src/Services/User/Core/User.Application/"]
COPY ["src/Shared/BuildingBlocks/BuildingBlocks.csproj", "src/Shared/BuildingBlocks/"]
COPY ["src/Shared/Common/Common.csproj", "src/Shared/Common/"]
COPY ["src/Shared/EventSourcing/EventSourcing.csproj", "src/Shared/EventSourcing/"]
COPY ["src/Services/User/Core/User.Domain/User.Domain.csproj", "src/Services/User/Core/User.Domain/"]
COPY ["src/Services/User/Core/User.Infrastructure/User.Infrastructure.csproj", "src/Services/User/Core/User.Infrastructure/"]
COPY ["src/Directory.Build.props", "src/"]
COPY ["src/Directory.Packages.props", "src/"]
RUN dotnet restore "src/Services/User/Api/User.Api/User.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/User/Api/User.Api"
RUN dotnet build "./User.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./User.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "User.Api.dll"]

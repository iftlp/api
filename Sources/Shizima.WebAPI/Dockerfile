FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG NUGET_FEED_URL="https://gitlab.isima.fr/api/v4/groups/11770/-/packages/nuget/index.json"
ARG NUGET_FEED_NAME="gitlab.isima"
ARG NUGET_PASSWORD_2

WORKDIR /src
COPY ["Shizima.WebAPI/Shizima.WebAPI.csproj", "Shizima.WebAPI/"]
COPY ["Shizima.Providers/Shizima.Providers.csproj", "Shizima.Providers/"]
COPY ["Shizima.Entities/Shizima.Entities.csproj", "Shizima.Entities/"]
COPY ["Shizima.Business/Shizima.Business.csproj", "Shizima.Business/"]
COPY ["Shizima.Dtos/Shizima.Dtos.csproj", "Shizima.Dtos/"]
COPY ["Shizima.WebAPI/Migrations", "Shizima.WebAPI/Migrations"]
COPY ["Shizima.MinioClient/Shizima.MinioClient.csproj", "Shizima.MinioClient/"]

RUN dotnet nuget add source $NUGET_FEED_URL --name $NUGET_FEED_NAME --username gitlab+deploy-token-38 --password $NUGET_PASSWORD_2 --store-password-in-clear-text
RUN dotnet restore "Shizima.WebAPI/Shizima.WebAPI.csproj"
COPY . .
WORKDIR "/src/Shizima.WebAPI"
RUN dotnet build "Shizima.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Shizima.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
RUN mkdir -p /app/publish/Migrations
COPY --from=build /src/Shizima.WebAPI/Migrations /app/publish/Migrations

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish /app/publish/Migrations ./Migrations

ENTRYPOINT ["dotnet", "Shizima.WebAPI.dll"]

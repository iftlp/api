FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Shizima.WebAPI/Shizima.WebAPI.csproj", "Shizima.WebAPI/"]
COPY ["Shizima.Providers/Shizima.Providers.csproj", "Shizima.Providers/"]
COPY ["Shizima.Entities/Shizima.Entities.csproj", "Shizima.Entities/"]
COPY ["Shizima.Business/Shizima.Business.csproj", "Shizima.Business/"]
COPY ["Shizima.Dtos/Shizima.Dtos.csproj", "Shizima.Dtos/"]
RUN dotnet restore "Shizima.WebAPI/Shizima.WebAPI.csproj"
COPY . .
WORKDIR "/src/Shizima.WebAPI"
RUN dotnet build "Shizima.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Shizima.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Shizima.WebAPI.dll"]

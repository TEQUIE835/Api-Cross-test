FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["userManagement.Api/userManagement.Api.csproj", "userManagement.Api/"]
COPY ["userManagement.Application/userManagement.Application.csproj", "userManagement.Application/"]
COPY ["userManagement.Domain/userManagement.Domain.csproj", "userManagement.Domain/"]
COPY ["userManagement.Infrastructure/userManagement.Infrastructure.csproj", "userManagement.Infrastructure/"]
RUN dotnet restore "userManagement.Api/userManagement.Api.csproj"
COPY . .
WORKDIR "/src/userManagement.Api"
RUN dotnet build "./userManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./userManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ["dotnet", "userManagement.Api.dll"]
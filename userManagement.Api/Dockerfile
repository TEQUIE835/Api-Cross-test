# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar archivos de proyecto primero (para cachear el restore)
COPY *.sln ./
COPY userManagement.Api/*.csproj userManagement.Api/
COPY userManagement.Application/*.csproj userManagement.Application/
COPY userManagement.Domain/*.csproj userManagement.Domain/
COPY userManagement.Infrastructure/*.csproj userManagement.Infrastructure/
COPY Certs /app/Certs

RUN dotnet restore userManagement.sln

# Copiar el resto del código
COPY . .

RUN dotnet publish userManagement.Api/userManagement.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ARG MYSQL_CONNECTION_STRING
ENV ConnectionStrings__Default=$MYSQL_CONNECTION_STRING

# Guardar el certificado desde variable de entorno
ARG MYSQL_CA_PEM
ENV MYSQL_CA_PEM=$MYSQL_CA_PEM

# Crear el archivo del certificado en tiempo de build
RUN mkdir -p /app/Certs && \
    if [ -n "$MYSQL_CA_PEM" ]; then echo "$MYSQL_CA_PEM" > /app/Certs/ca.pem; fi


ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "userManagement.Api.dll"]

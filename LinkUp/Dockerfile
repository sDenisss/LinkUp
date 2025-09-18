# Базовый runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["LinkUp.csproj", "./"]
RUN dotnet restore "LinkUp.csproj"
COPY . .
RUN dotnet build "LinkUp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LinkUp.csproj" -c Release -o /app/publish

# Финальный образ
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LinkUp.dll"]

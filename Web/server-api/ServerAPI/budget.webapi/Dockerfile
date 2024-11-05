# Используем базовый образ для .NET 7.0
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

# Используем базовый образ для сборки .NET приложения
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Копируем файлы проекта и восстанавливаем зависимости
COPY ["WEB/server-api/ServerAPI/budget.webapi.csproj", "WEB/server-api/ServerAPI/"]
RUN dotnet restore "WEB/server-api/ServerAPI/budget.webapi.csproj"

# Копируем все остальные файлы и собираем приложение
COPY . .
WORKDIR "/src/WEB/server-api/ServerAPI"
RUN dotnet build "budget.webapi.csproj" -c Release -o /app/build

# Публикуем приложение
FROM build AS publish
RUN dotnet publish "budget.webapi.csproj" -c Release -o /app/publish

# Используем базовый образ для запуска приложения
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "budget.webapi.dll"]

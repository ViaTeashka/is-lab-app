 # 1. Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем ТОЛЬКО файлы основного проекта
COPY IsLabApp.csproj .
RUN dotnet restore IsLabApp.csproj

# Копируем исходники основного проекта (игнорируя всё остальное)
COPY . .

# Удаляем ВООБЩЕ ВСЁ, что не касается сборки, перед публикацией
RUN rm -rf IsLabApp.Tests obj bin

# Собираем строго один проект
RUN dotnet publish IsLabApp.csproj -c Release -o /app/publish /p:UseAppHost=false

# 2. Этап запуска
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "IsLabApp.dll"]

# 1. Этап сборки (Build stage)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файл проекта и восстанавливаем зависимости
COPY ["IsLabApp.csproj", "./"]
RUN dotnet restore "IsLabApp.csproj"

# Копируем всё остальное
COPY . .

# УДАЛЯЕМ МУСОР (obj/bin), который мешает сборке
RUN rm -rf obj/ bin/ IsLabApp.Tests/obj/ IsLabApp.Tests/bin/

# Собираем только основной проект
RUN dotnet publish "IsLabApp.csproj" -c Release -o /app/publish

# 2. Этап запуска (Runtime stage)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Выставляем порт и запускаем
EXPOSE 8080
ENTRYPOINT ["dotnet", "IsLabApp.dll"]


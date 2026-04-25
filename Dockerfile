# этап build: восстановление пакетов, сборка, публикация
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# копировать только нужные файлы
COPY *.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish

# этап runtime: запуск опубликованного приложения
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# копировать только нужные файлы из этапа сборки
COPY --from=build /app/publish .

# выставить порт приложения 8080
EXPOSE 8080

# запускать dotnet <app>.dll
ENTRYPOINT ["dotnet", "IsLabApp.dll"]

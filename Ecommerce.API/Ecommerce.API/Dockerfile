FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore

RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0

RUN apt-get update && apt-get install -y curl

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Ecommerce.API.dll"]
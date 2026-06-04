FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY . .

RUN dotnet restore OlimpBack.csproj

RUN dotnet publish OlimpBack.csproj \
    -c Release \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5154

ENTRYPOINT ["dotnet", "OlimpBack.dll"]
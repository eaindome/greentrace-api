FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY GreenTrace.Server/GreenTrace.Server.csproj GreenTrace.Server/
RUN dotnet restore GreenTrace.Server/GreenTrace.Server.csproj

COPY GreenTrace.Server/ GreenTrace.Server/
RUN dotnet publish GreenTrace.Server/GreenTrace.Server.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080} exec dotnet GreenTrace.Server.dll"]

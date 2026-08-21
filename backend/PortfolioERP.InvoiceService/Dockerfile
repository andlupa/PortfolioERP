FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY PortfolioERP.InvoiceService.csproj .

RUN dotnet restore PortfolioERP.InvoiceService.csproj

COPY . .

RUN dotnet publish PortfolioERP.InvoiceService.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "PortfolioERP.InvoiceService.dll"]
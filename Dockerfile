FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["backend/PortfolioERP.Api/PortfolioERP.Api.csproj", "PortfolioERP.Api/"]
COPY ["backend/PortfolioERP.Application/PortfolioERP.Application.csproj", "PortfolioERP.Application/"]
COPY ["backend/PortfolioERP.Domain/PortfolioERP.Domain.csproj", "PortfolioERP.Domain/"]
COPY ["backend/PortfolioERP.Infrastructure/PortfolioERP.Infrastructure.csproj", "PortfolioERP.Infrastructure/"]

RUN dotnet restore "PortfolioERP.Api/PortfolioERP.Api.csproj"

COPY backend/ .

WORKDIR /src/PortfolioERP.Api

RUN dotnet publish "PortfolioERP.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "PortfolioERP.Api.dll"]
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["PromoCodeFactory.WebHost/PromoCodeFactory.WebHost.csproj", "PromoCodeFactory.WebHost/"]
COPY ["PromoCodeFactory.DataAccess/PromoCodeFactory.DataAccess.csproj", "PromoCodeFactory.DataAccess/"]
COPY ["PromoCodeFactory.Core/PromoCodeFactory.Core.csproj", "PromoCodeFactory.Core/"]

RUN dotnet restore "PromoCodeFactory.Core/PromoCodeFactory.Core.csproj"

COPY . .
WORKDIR /src/PromoCodeFactory.WebHost
RUN dotnet publish "PromoCodeFactory.WebHost.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PromoCodeFactory.WebHost.dll"]
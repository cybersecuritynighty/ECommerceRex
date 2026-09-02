FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["ECommerceRex/ECommerceRex.csproj", "ECommerceRex/"]
RUN dotnet restore "ECommerceRex/ECommerceRex.csproj"
COPY . .
WORKDIR "ECommerceRex"
RUN dotnet build "ECommerceRex.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ECommerceRex.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ECommerceRex.dll"]

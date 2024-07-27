FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DecorStore.API/DecorStore.API.csproj", "DecorStore.API/"]
COPY ["DecorStore.BL/DecorStore.BL.csproj", "DecorStore.BL/"]
COPY ["DecorStore.Data/DecorStore.Data.csproj", "DecorStore.Data/"]
RUN dotnet restore "./DecorStore.API/DecorStore.API.csproj"
COPY . .
WORKDIR "/src/DecorStore.API"
RUN dotnet build "./DecorStore.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./DecorStore.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DecorStore.API.dll"]

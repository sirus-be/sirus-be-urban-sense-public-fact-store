#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Components/FactStore/FactStore.Jobs/FactStore.Jobs.csproj", "Components/FactStore/FactStore.Jobs/"]
RUN dotnet restore "Components/FactStore/FactStore.Jobs/FactStore.Jobs.csproj"
COPY . .
WORKDIR "/src/Components/FactStore/FactStore.Jobs"
RUN dotnet build "FactStore.Jobs.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FactStore.Jobs.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FactStore.Jobs.dll"]
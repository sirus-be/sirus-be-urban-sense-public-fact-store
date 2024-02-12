#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["FactStore.Portal/FactStore.Portal.csproj", "FactStore.Portal/"]
COPY ["FactStore.Client/FactStore.Client.csproj", "FactStore.Client/"]
COPY ["FactStore.Models/FactStore.Models.csproj", "FactStore.Models/"]
COPY ["Core/Core.csproj", "Core/"]
COPY ["Shared.Portal/Shared.Portal.csproj", "Shared.Portal/"]
RUN dotnet restore "FactStore.Portal/FactStore.Portal.csproj"
COPY . .
WORKDIR "/src/FactStore.Portal"
RUN dotnet build "FactStore.Portal.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FactStore.Portal.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FactStore.Portal.dll"]
#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["src/Shortner.Api/Shortner.Api.csproj", "src/Shortner.Api/"]
COPY ["src/Shortner.Core/Shortner.Core.csproj", "src/Shortner.Core/"]
RUN dotnet restore "src/Shortner.Api/Shortner.Api.csproj"
COPY . .
WORKDIR "/src/src/Shortner.Api"
RUN dotnet build "Shortner.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Shortner.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Shortner.Api.dll"]
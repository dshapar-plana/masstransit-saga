#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["MassTransitSagaDeadlock.Console.Client/MassTransitSagaDeadlock.Console.Client.csproj", "MassTransitSagaDeadlock.Console.Client/"]
RUN dotnet restore "MassTransitSagaDeadlock.Console.Client/MassTransitSagaDeadlock.Console.Client.csproj"
COPY . .
WORKDIR "/src/MassTransitSagaDeadlock.Console.Client"
RUN dotnet build "MassTransitSagaDeadlock.Console.Client.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MassTransitSagaDeadlock.Console.Client.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MassTransitSagaDeadlock.Console.Client.dll"]
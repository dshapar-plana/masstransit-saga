FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["MassTransitSagaDeadlock.Worker/MassTransitSagaDeadlock.Worker.csproj", "MassTransitSagaDeadlock.Worker/"]
RUN dotnet restore "MassTransitSagaDeadlock.Worker/MassTransitSagaDeadlock.Worker.csproj"
COPY . .
WORKDIR "/src/MassTransitSagaDeadlock.Worker"
RUN dotnet build "MassTransitSagaDeadlock.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MassTransitSagaDeadlock.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MassTransitSagaDeadlock.Worker.dll"]
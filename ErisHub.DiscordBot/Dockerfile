#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["ErisHub.DiscordBot/ErisHub.DiscordBot.csproj", "ErisHub.DiscordBot/"]
COPY ["ErisHub.Shared/ErisHub.Shared.csproj", "ErisHub.Shared/"]
COPY ["ErisHub/ErisHub.csproj", "ErisHub/"]
RUN dotnet restore "ErisHub.DiscordBot/ErisHub.DiscordBot.csproj"
COPY . .
WORKDIR "/src/ErisHub.DiscordBot"
RUN dotnet build "ErisHub.DiscordBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ErisHub.DiscordBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ErisHub.DiscordBot.dll", "--environment=Production"]
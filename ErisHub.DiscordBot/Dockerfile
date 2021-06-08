#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ErisHub.DiscordBot/ErisHub.DiscordBot.csproj", "ErisHub.DiscordBot/"]
COPY ["ErisHub.Shared/ErisHub.Shared.csproj", "ErisHub.Shared/"]
COPY ["ErisHub/ErisHub.csproj", "ErisHub/"]
RUN dotnet restore "ErisHub.DiscordBot/ErisHub.DiscordBot.csproj"
COPY . .
WORKDIR "/src/ErisHub.DiscordBot"

# Release configuration

FROM build AS publish
RUN dotnet publish "ErisHub.DiscordBot.csproj" -c Release -o /app/publish

FROM base AS release
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ErisHub.DiscordBot.dll", "--environment=Production"]

# Debug configuration

FROM build AS build-debug
RUN dotnet restore "../ErisHub/ErisHub.csproj"
RUN dotnet build -c Debug -o /app/Debug

FROM base AS debug
WORKDIR /app
COPY --from=build-debug /app/Debug .
ENTRYPOINT ["dotnet", "ErisHub.DiscordBot.dll", "--environment=Development"]
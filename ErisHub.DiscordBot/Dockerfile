FROM mcr.microsoft.com/dotnet/core/runtime:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 62907
EXPOSE 44303

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["ErisHub.DiscordBot/ErisHub.DiscordBot.csproj", "ErisHub.DiscordBot/"]
COPY ["ErisHub.Shared/ErisHub.Shared.csproj", "ErisHub.Shared/"]
COPY ["ErisHub/ErisHub.csproj", "ErisHub/"]
RUN dotnet restore "ErisHub.DiscordBot/ErisHub.DiscordBot.csproj"
COPY . .
WORKDIR "/src/ErisHub.DiscordBot"
RUN dotnet build "ErisHub.DiscordBot.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ErisHub.DiscordBot.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ErisHub.DiscordBot.dll", "--environment=Production"]
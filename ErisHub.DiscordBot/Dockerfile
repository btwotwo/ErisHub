FROM microsoft/dotnet:2.1-runtime AS base
WORKDIR /app
EXPOSE 62907
EXPOSE 44303

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["ErisHub.DiscordBot/ErisHub.DiscordBot.csproj", "ErisHub.DiscordBot/"]
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
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 62907
EXPOSE 44303

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["ErisHub/ErisHub.csproj", "ErisHub/"]
COPY ["ErisHub.Shared/ErisHub.Shared.csproj", "ErisHub.Shared/"]
RUN dotnet restore "ErisHub/ErisHub.csproj"
COPY . .
WORKDIR "/src/ErisHub"
RUN dotnet build "ErisHub.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ErisHub.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ErisHub.dll", "--environment=Production"]

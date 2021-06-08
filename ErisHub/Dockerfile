FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 62907
EXPOSE 44303

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS restore
WORKDIR /src
COPY ["ErisHub/ErisHub.csproj", "ErisHub/"]
COPY ["ErisHub.Shared/ErisHub.Shared.csproj", "ErisHub.Shared/"]
RUN dotnet restore "ErisHub/ErisHub.csproj"
COPY . .
WORKDIR "/src/ErisHub"

# Release configuration

FROM restore AS publish
RUN dotnet publish "ErisHub.csproj" -c Release -o /app

FROM base AS release
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ErisHub.dll"]


# Debug configuration

FROM restore AS build-debug
RUN dotnet build "ErisHub.csproj" -c Debug -o /app


FROM base AS debug
WORKDIR /app
COPY --from=build-debug /app .
ENTRYPOINT ["dotnet", "ErisHub.dll"]

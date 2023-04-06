#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM azhe403/dotnet-sdk-node:latest AS build
WORKDIR /backend
COPY ["Directory.Build.props", "."]
COPY ["backend/ZiziBot.Engine/ZiziBot.Engine.csproj", "backend/ZiziBot.Engine/"]
COPY ["backend/ZiziBot.Allowed.TelegramBot/ZiziBot.Allowed.TelegramBot.csproj", "backend/ZiziBot.Allowed.TelegramBot/"]
COPY ["backend/ZiziBot.Application/ZiziBot.Application.csproj", "backend/ZiziBot.Application/"]
COPY ["backend/ZiziBot.Exceptions/ZiziBot.Exceptions.csproj", "backend/ZiziBot.Exceptions/"]
COPY ["backend/ZiziBot.Parsers/ZiziBot.Parsers.csproj", "backend/ZiziBot.Parsers/"]
COPY ["backend/ZiziBot.Contracts/ZiziBot.Contracts.csproj", "backend/ZiziBot.Contracts/"]
COPY ["backend/ZiziBot.Utils/ZiziBot.Utils.csproj", "backend/ZiziBot.Utils/"]
COPY ["backend/ZiziBot.DataSource/ZiziBot.DataSource.csproj", "backend/ZiziBot.DataSource/"]
COPY ["backend/ZiziBot.Hangfire/ZiziBot.Hangfire.csproj", "backend/ZiziBot.Hangfire/"]
COPY ["backend/ZiziBot.DiscordNet.DiscordBot/ZiziBot.DiscordNet.DiscordBot.csproj", "backend/ZiziBot.DiscordNet.DiscordBot/"]
COPY ["backend/ZiziBot.Infrastructure/ZiziBot.Infrastructure.csproj", "backend/ZiziBot.Infrastructure/"]
COPY ["backend/ZiziBot.Caching/ZiziBot.Caching.csproj", "backend/ZiziBot.Caching/"]
COPY ["backend/ZiziBot.Kot.MongoMigrations/ZiziBot.Kot.MongoMigrations.csproj", "backend/ZiziBot.Kot.MongoMigrations/"]
COPY ["backend/ZiziBot.WebApi/ZiziBot.WebApi.csproj", "backend/ZiziBot.WebApi/"]
RUN dotnet restore "backend/ZiziBot.Engine/ZiziBot.Engine.csproj"
COPY . .

WORKDIR "/backend"
RUN dotnet run --project "backend/ZiziBot.Cli/ZiziBot.Cli.csproj"  --toolName=UpdateVersion --mode=DependsOnCondition

WORKDIR "/backend/backend/ZiziBot.Engine"
RUN dotnet build "ZiziBot.Engine.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ZiziBot.Engine.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZiziBot.Engine.dll"]
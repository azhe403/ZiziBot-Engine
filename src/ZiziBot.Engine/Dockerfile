#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM azhe403/dotnet-sdk-node:latest AS build
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["src/ZiziBot.Engine/ZiziBot.Engine.csproj", "src/ZiziBot.Engine/"]
COPY ["src/ZiziBot.Allowed.TelegramBot/ZiziBot.Allowed.TelegramBot.csproj", "src/ZiziBot.Allowed.TelegramBot/"]
COPY ["src/ZiziBot.Application/ZiziBot.Application.csproj", "src/ZiziBot.Application/"]
COPY ["src/ZiziBot.Exceptions/ZiziBot.Exceptions.csproj", "src/ZiziBot.Exceptions/"]
COPY ["src/ZiziBot.Parsers/ZiziBot.Parsers.csproj", "src/ZiziBot.Parsers/"]
COPY ["src/ZiziBot.Contracts/ZiziBot.Contracts.csproj", "src/ZiziBot.Contracts/"]
COPY ["src/ZiziBot.Utils/ZiziBot.Utils.csproj", "src/ZiziBot.Utils/"]
COPY ["src/ZiziBot.DataSource/ZiziBot.DataSource.csproj", "src/ZiziBot.DataSource/"]
COPY ["src/ZiziBot.Hangfire/ZiziBot.Hangfire.csproj", "src/ZiziBot.Hangfire/"]
COPY ["src/ZiziBot.DiscordNet.DiscordBot/ZiziBot.DiscordNet.DiscordBot.csproj", "src/ZiziBot.DiscordNet.DiscordBot/"]
COPY ["src/ZiziBot.Infrastructure/ZiziBot.Infrastructure.csproj", "src/ZiziBot.Infrastructure/"]
COPY ["src/ZiziBot.Caching/ZiziBot.Caching.csproj", "src/ZiziBot.Caching/"]
COPY ["src/ZiziBot.Kot.MongoMigrations/ZiziBot.Kot.MongoMigrations.csproj", "src/ZiziBot.Kot.MongoMigrations/"]
COPY ["src/ZiziBot.WebApi/ZiziBot.WebApi.csproj", "src/ZiziBot.WebApi/"]
RUN dotnet restore "src/ZiziBot.Engine/ZiziBot.Engine.csproj"
COPY . .
WORKDIR "/src/src/ZiziBot.Engine"
RUN dotnet build "ZiziBot.Engine.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ZiziBot.Engine.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZiziBot.Engine.dll"]
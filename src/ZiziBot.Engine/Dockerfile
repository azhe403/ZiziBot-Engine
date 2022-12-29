FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Install NodeJs
RUN apt-get update && \
apt-get install -y wget && \
apt-get install -y gnupg2 && \
wget -qO- https://deb.nodesource.com/setup_18.x | bash - && \
apt-get install -y build-essential nodejs
# End Install

COPY ["src/ZiziBot.Engine/ZiziBot.Engine.csproj", "src/ZiziBot.Engine/"]
COPY ["src/ZiziBot.Application/ZiziBot.Application.csproj", "src/ZiziBot.Application/"]
COPY ["src/ZiziBot.Hangfire/ZiziBot.Hangfire.csproj", "src/ZiziBot.Hangfire/"]
COPY ["src/ZiziBot.Contracts/ZiziBot.Contracts.csproj", "src/ZiziBot.Contracts/"]
COPY ["src/ZiziBot.Utils/ZiziBot.Utils.csproj", "src/ZiziBot.Utils/"]
COPY ["src/ZiziBot.Infrastructure/ZiziBot.Infrastructure.csproj", "src/ZiziBot.Infrastructure/"]
COPY ["src/ZiziBot.Caching/ZiziBot.Caching.csproj", "src/ZiziBot.Caching/"]
COPY ["src/ng-dashboard","src/ng-dashboard"]

RUN dotnet restore "src/ZiziBot.Engine/ZiziBot.Engine.csproj"
COPY . .
WORKDIR "/src/src/ZiziBot.Engine"
RUN dotnet build "ZiziBot.Engine.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ZiziBot.Engine.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZiziBot.Engine.dll"]

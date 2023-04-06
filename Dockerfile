#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM azhe403/dotnet-sdk-node:latest AS build
WORKDIR /build
COPY . .
RUN dotnet restore "backend/ZiziBot.Engine/ZiziBot.Engine.csproj"
RUN dotnet run --project "backend/ZiziBot.Cli/ZiziBot.Cli.csproj"  --toolName=UpdateVersion --mode=DependsOnCondition

WORKDIR "/build/backend/ZiziBot.Engine"
RUN dotnet build "ZiziBot.Engine.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ZiziBot.Engine.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZiziBot.Engine.dll"]
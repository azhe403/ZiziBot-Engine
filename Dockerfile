#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

ARG DOTNET_TAG=10.0-alpine
ARG APP_VERSION=1.0.0.0

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
WORKDIR /app
RUN apk add --no-cache icu-data-full icu-libs
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:latest AS build
WORKDIR /build
COPY . .
RUN git submodule update --init --recursive
RUN dotnet restore "backend/ZiziBot.Engine/ZiziBot.Engine.csproj"

WORKDIR "/build/backend/ZiziBot.Engine"
RUN dotnet build "ZiziBot.Engine.csproj" -c Release -o /app/build

FROM build AS publish
ARG APP_VERSION
RUN dotnet publish "ZiziBot.Engine.csproj" -c Release -o /app/publish /p:UseAppHost=false /p:Version=$APP_VERSION

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZiziBot.Engine.dll"]
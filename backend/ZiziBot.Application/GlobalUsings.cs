// Global using directives

global using JetBrains.Annotations;
global using MediatR;
global using ZiziBot.Application.Core;
global using ZiziBot.Application.Handlers.Telegram.Core;
global using ZiziBot.Application.Handlers.Telegram.Data;
global using ZiziBot.Application.Handlers.Telegram.Rss;
global using ZiziBot.Application.Handlers.Telegram.Security;
global using ZiziBot.Application.Handlers.Telegram.WebHook.GitHub;
global using ZiziBot.Application.Services;
global using ZiziBot.Application.Utils;
global using ZiziBot.Contracts;
global using ZiziBot.Contracts.Allowed.Models;
global using ZiziBot.Contracts.Configs;
global using ZiziBot.Contracts.Constants;
global using ZiziBot.Contracts.Dtos;
global using ZiziBot.Contracts.Enums;
global using ZiziBot.Contracts.Interfaces;
global using ZiziBot.Contracts.Types;
global using ZiziBot.Contracts.Vendor.FathimahApi;
global using ZiziBot.DataSource.MongoDb;
global using ZiziBot.DataSource.MongoDb.Entities;
global using ZiziBot.DataSource.Repository;
global using ZiziBot.Exceptions;
global using ZiziBot.Parsers;
global using ZiziBot.Parsers.WebParser;
global using ZiziBot.Utils;
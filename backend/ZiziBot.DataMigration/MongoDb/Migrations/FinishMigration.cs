﻿using MongoDB.Driver;
using ZiziBot.DataMigration.MongoDb.Interfaces;

namespace ZiziBot.DataMigration.MongoDb.Migrations;

public class FinishMigration : IPostMigration
{
    public async Task UpAsync(IMongoDatabase database)
    { }

    public async Task DownAsync(IMongoDatabase database)
    { }
}
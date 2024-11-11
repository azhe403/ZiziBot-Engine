using MongoDB.Bson;
using MongoDB.Driver;
using ZiziBot.Contracts.Enums;
using ZiziBot.DataMigration.MongoDb.Interfaces;

namespace ZiziBot.DataMigration.MongoDb.Migrations;

public class MirrorApprovalUpdateSourceMigration : IPostMigration
{
    public async Task UpAsync(IMongoDatabase database)
    {
        var collection = database.GetCollection<BsonDocument>("MirrorApproval");
        var filter = Builders<BsonDocument>.Filter.Empty;
        var listMirrorApproval = await collection.Find(filter).ToListAsync();
        var update = Builders<BsonDocument>.Update.Set("UpdatedDate", DateTime.UtcNow);

        foreach (var approval in listMirrorApproval)
        {
            var source = DonationSource.Unknown;
            if (approval["PaymentUrl"] != BsonNull.Value)
            {
                var paymentUrl = approval["PaymentUrl"].AsString;

                if (paymentUrl.Contains("trakteer"))
                {
                    source = DonationSource.Trakteer;
                }
                else if (paymentUrl.Contains("saweria"))
                {
                    source = DonationSource.Saweria;
                }

                update.Set("UpdatedDate", DateTime.UtcNow).Set("Source", source);
                await collection.UpdateOneAsync(Builders<BsonDocument>.Filter.Eq("_id", approval["_id"]), update);
            }
            else if (approval["PaymentUrl"] == BsonNull.Value)
            {
                update.Set("PaymentUrl", "https://p.azhe.my.id").Set("Source", DonationSource.Unknown);
                await collection.UpdateOneAsync(Builders<BsonDocument>.Filter.Eq("_id", approval["_id"]), update);
            }
        }
    }

    public async Task DownAsync(IMongoDatabase database)
    { }
}
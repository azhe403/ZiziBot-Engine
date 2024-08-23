using MongoDB.Driver;

namespace ZiziBot.DataSource.Utils;

public static class DbUtil
{
    public static MongoUrl ToMongoUrl(this string mongodbConnectionString)
    {
        var url = new MongoUrl(mongodbConnectionString);
        return url;
    }
}
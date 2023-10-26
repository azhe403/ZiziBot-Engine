using MongoFramework.Linq;

namespace ZiziBot.DataSource.Repository;

public class AdditionalRepository
{
    private readonly MongoDbContextBase _mongoDbContextBase;

    public AdditionalRepository(MongoDbContextBase mongoDbContextBase)
    {
        _mongoDbContextBase = mongoDbContextBase;
    }

    public async Task<TonjooAwbEntity?> GetStoredAwb(string courier, string awb)
    {
        var awbInfo = await _mongoDbContextBase.TonjooAwb
            .Where(entity => entity.Awb == awb)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync();

        return awbInfo;
    }

    public async Task SaveAwbInfo(TonjooAwbDetail detail)
    {
        _mongoDbContextBase.TonjooAwb.Add(new TonjooAwbEntity()
        {
            Awb = detail.Code,
            Courier = detail.Kurir.FirstOrDefault("N/A").ToLower(),
            Detail = detail,
            Status = (int)EventStatus.Complete
        });

        await _mongoDbContextBase.SaveChangesAsync();
    }
}
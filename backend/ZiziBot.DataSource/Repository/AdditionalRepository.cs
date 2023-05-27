using MongoFramework.Linq;

namespace ZiziBot.DataSource.Repository;

public class AdditionalRepository
{
    private readonly AdditionalDbContext _additionalDbContext;

    public AdditionalRepository(AdditionalDbContext additionalDbContext)
    {
        _additionalDbContext = additionalDbContext;
    }

    public async Task<TonjooAwbEntity?> GetStoredAwb(string courier, string awb)
    {
        var awbInfo = await _additionalDbContext.TonjooAwb
            .Where(entity => entity.Awb == awb)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync();

        return awbInfo;
    }

    public async Task SaveAwbInfo(TonjooAwbDetail detail)
    {
        _additionalDbContext.TonjooAwb.Add(new TonjooAwbEntity()
        {
            Awb = detail.Code,
            Courier = detail.Kurir.FirstOrDefault("N/A").ToLower(),
            Detail = detail,
            Status = (int)EventStatus.Complete
        });

        await _additionalDbContext.SaveChangesAsync();
    }
}
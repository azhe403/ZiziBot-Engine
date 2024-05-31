using Flurl;
using Flurl.Http;
using ZiziBot.Types.Vendor.Subdl;

namespace ZiziBot.Services;

public class SubdlService
{
    public async Task<Popular> FetchPopular()
    {
        var response = await UrlConst.API_SUBDL_BASE.AppendPathSegment("popular").GetJsonAsync<Popular>();

        return response;
    }
}
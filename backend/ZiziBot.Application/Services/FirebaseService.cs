using Firebase.Database;
using Firebase.Database.Query;
using Google.Apis.Auth.OAuth2;

namespace ZiziBot.Application.Services;

public class FirebaseService
{
    private readonly AppSettingRepository _appSettingRepository;
    private GcpConfig? _gcpConfig;

    public FirebaseService(AppSettingRepository appSettingRepository)
    {
        _appSettingRepository = appSettingRepository;
    }

    public async Task PostAsync<T>(string resourceName, T data)
    {
        _gcpConfig = await _appSettingRepository.GetConfigSection<GcpConfig>();

        if (_gcpConfig == null) return;

        using var client = new FirebaseClient(
            _gcpConfig.FirebaseProjectUrl,
            new FirebaseOptions
            {
                AuthTokenAsyncFactory = GetAccessToken,
                AsAccessToken = true
            }
        );

        await client
            .Child(resourceName)
            .PostAsync(data);

    }

    private async Task<string> GetAccessToken()
    {
        var credential = GoogleCredential.FromJson(_gcpConfig?.FirebaseServiceAccountJson)
            .CreateScoped("https://www.googleapis.com/auth/firebase.database", "https://www.googleapis.com/auth/userinfo.email");

        var accessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();

        return accessToken;
    }
}
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Database.Streaming;
using Google.Apis.Auth.OAuth2;

namespace ZiziBot.Services;

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
        var client = await GetClient();

        await client.Child(resourceName).PostAsync(data);
    }

    public async Task PutAsync<T>(string resourceName, T data)
    {
        var client = await GetClient();

        await client.Child(resourceName).PutAsync(data);
    }

    public async Task DeleteAsync(string resourceName)
    {
        var client = await GetClient();

        await client.Child(resourceName).DeleteAsync();
    }

    public async Task<IObservable<FirebaseEvent<T>>> AsObservable<T>(string resourceName)
    {
        var client = await GetClient();

        return client.Child(resourceName).AsObservable<T>();
    }

    private async Task<FirebaseClient> GetClient()
    {
        _gcpConfig = await _appSettingRepository.GetConfigSectionAsync<GcpConfig>();

        if (_gcpConfig == null)
            throw new AppException("Firebase config not found");

        var client = new FirebaseClient(
            baseUrl: _gcpConfig.FirebaseProjectUrl,
            options: new FirebaseOptions
            {
                AuthTokenAsyncFactory = GetAccessToken,
                AsAccessToken = true
            }
        );

        return client;
    }

    private async Task<string> GetAccessToken()
    {
        var credential = GoogleCredential.FromJson(_gcpConfig?.FirebaseServiceAccountJson)
            .CreateScoped("https://www.googleapis.com/auth/firebase.database", "https://www.googleapis.com/auth/userinfo.email");

        var accessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();

        return accessToken;
    }
}
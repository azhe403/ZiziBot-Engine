using Firebase.Database;
using Firebase.Database.Query;
using Google.Apis.Auth.OAuth2;

namespace ZiziBot.Application.Services;

public class FirebaseService
{
	private readonly string _baseUrl;

	public FirebaseService()
	{
		_baseUrl = "";
	}

	public async Task SendAsync<T>(string resourceName, T data)
	{
		using var client = new FirebaseClient(
			_baseUrl,
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

		var credential = GoogleCredential.FromFile(@"")
			.CreateScoped(
				new[]
				{
					"https://www.googleapis.com/auth/firebase.database",
					"https://www.googleapis.com/auth/userinfo.email"
				}
			);

		var accessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();

		return accessToken;
	}
}
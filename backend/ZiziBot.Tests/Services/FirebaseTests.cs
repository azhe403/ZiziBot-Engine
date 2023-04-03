using Xunit;

namespace ZiziBot.Tests.Services;

public class FirebaseTests
{
	private readonly FirebaseService _firebaseService;

	public FirebaseTests(FirebaseService firebaseService)
	{
		_firebaseService = firebaseService;
	}

	[Fact]
	public void Save()
	{
		var row = new
		{
			Name = "Fulan"
		};

		var json = row;

		_firebaseService.SendAsync(Guid.NewGuid().ToString(), json).Wait();
	}
}
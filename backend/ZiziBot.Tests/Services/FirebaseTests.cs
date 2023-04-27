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
    public async Task FirebasePostTest()
    {
        var row = new
        {
            Name = "Fulan"
        };


        await _firebaseService.PostAsync("test/post/" + Guid.NewGuid(), row);

        Assert.True(true);
    }
}
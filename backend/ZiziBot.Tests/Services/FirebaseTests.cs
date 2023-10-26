using Xunit;

namespace ZiziBot.Tests.Services;

public class FirebaseTests : IAsyncLifetime
{
    private readonly FirebaseService _firebaseService;

    public FirebaseTests(FirebaseService firebaseService)
    {
        _firebaseService = firebaseService;
    }

    [Fact]
    public async Task FirebasePostTest()
    {
        var data = new
        {
            Name = "Fulan"
        };

        var path = "test/post/" + Guid.NewGuid();

        await _firebaseService.PostAsync(path, data);

        Assert.True(true);
    }

    [Fact]
    public async Task FirebasePutTest()
    {
        var data = new
        {
            Name = "Fulan",
            Oid = Guid.NewGuid()
        };

        var path = "test/put";

        await _firebaseService.PutAsync(path, data);

        Assert.True(true);
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _firebaseService.DeleteAsync("test");
    }
}
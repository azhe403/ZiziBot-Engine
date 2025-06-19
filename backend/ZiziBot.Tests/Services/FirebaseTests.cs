using Xunit;

namespace ZiziBot.Tests.Services;

public class FirebaseTests(FirebaseService firebaseService) : IAsyncLifetime
{
    [Fact]
    public async Task FirebasePostTest()
    {
        var data = new
        {
            Name = "Fulan"
        };

        var path = "test/post/" + Guid.NewGuid();

        await firebaseService.PostAsync(path, data);
        true.ShouldBeTrue();
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

        await firebaseService.PutAsync(path, data);
        true.ShouldBeTrue();
    }


    public async ValueTask DisposeAsync()
    {
        await firebaseService.DeleteAsync("test");
    }

    public ValueTask InitializeAsync()
    {
        throw new NotImplementedException();
    }
}
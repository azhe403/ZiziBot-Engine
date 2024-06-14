using FluentAssertions;
using Xunit;

namespace ZiziBot.Tests.Features;

public class MirrorPaymentTest
{
    public MirrorPaymentTest(MediatorService mediatorService)
    {
    }

    [Theory(Skip = "Deprecated")]
    [InlineData("ca9c28da-87c0-5d32-9b6f-a220d3d36dfd")]
    [InlineData("https://trakteer.id/payment-status/ca9c28da-87c0-5d32-9b6f-a220d3d36dfd")]
    public async Task TrakteerParserTest(string url)
    {
        var trakteerParsedDto = await url.ParseTrakteerWeb();

        trakteerParsedDto.RawText.Should().Contain("Pembayaran Berhasil");
        trakteerParsedDto.Cendols.Should().NotBeNullOrEmpty();
        trakteerParsedDto.AdminFees.Should().BeGreaterThan(0);
        trakteerParsedDto.Subtotal.Should().BeGreaterThan(0);
        trakteerParsedDto.OrderDate.Should().BeMoreThan(default);
        trakteerParsedDto.OrderId.Should().NotBeNullOrEmpty();
        trakteerParsedDto.PaymentMethod.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("https://trakteer.id/payment-status/ca9c28da-87c0-5d32-9b6f-a220d3d36dfdX")]
    public async Task TrakteerParserNegativeTest(string url)
    {
        var requiredNodes = await url.ParseTrakteerWeb();
        requiredNodes.RawText.Should().BeNullOrEmpty();
    }

    [Theory]
    [InlineData("ca9c28da-87c0-5d32-9b6f-a220d3d36dfd")]
    [InlineData("https://trakteer.id/payment-status/ca9c28da-87c0-5d32-9b6f-a220d3d36dfd")]
    public async Task TrakteerApiTest(string url)
    {
        var trakteerApi = await url.GetTrakteerApi();

        trakteerApi.OrderId.Should().NotBeNullOrEmpty();
        trakteerApi.CendolCount.Should().BeGreaterThan(0);
        trakteerApi.AdminFees.Should().BeGreaterThan(0);
        trakteerApi.Total.Should().BeGreaterThan(0);
        trakteerApi.OrderDate.Should().BeMoreThan(default);
        trakteerApi.PaymentMethod.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("9dd30a01-0c59-4cc4-9d53-b726f4579dc5")]
    [InlineData("https://saweria.co/receipt/9dd30a01-0c59-4cc4-9d53-b726f4579dc5")]
    public async Task SaweriaApiTest(string url)
    {
        var trakteerApi = await url.GetSaweriaApi();

        trakteerApi.OrderId.Should().NotBeNullOrEmpty();
        trakteerApi.CendolCount.Should().BeGreaterThan(0);
        trakteerApi.Total.Should().BeGreaterThan(0);
        trakteerApi.OrderDate.Should().BeMoreThan(default);
    }
}
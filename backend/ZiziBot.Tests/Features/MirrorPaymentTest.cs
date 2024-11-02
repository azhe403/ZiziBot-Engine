using FluentAssertions;
using Xunit;

namespace ZiziBot.Tests.Features;

public class MirrorPaymentTest(MediatorService mediatorService, MirrorPaymentService mirrorPaymentService)
{
    [Theory]
    [InlineData("ca9c28da-87c0-5d32-9b6f-a220d3d36dfd")] // trakteer
    [InlineData("65190576-e653-47d2-b472-9a367a54ed23")] // saweria
    public async Task ParseDonationTest(string url)
    {
        var donationParsedDto = await mirrorPaymentService.ParseTrakteerWeb(url);

        donationParsedDto.IsValid.Should().BeTrue();
        donationParsedDto.CendolCount.Should().BeGreaterThan(0);
        donationParsedDto.OrderDate.Should().BeMoreThan(default);
        donationParsedDto.OrderId.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("ca9c28da-87c0-5d32-9b6f-a220d3d36dfd")]
    [InlineData("https://trakteer.id/payment-status/ca9c28da-87c0-5d32-9b6f-a220d3d36dfd")]
    public async Task TrakteerParserTest(string url)
    {
        var trakteerParsedDto = await mirrorPaymentService.ParseTrakteerWeb(url);

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
        var requiredNodes = await mirrorPaymentService.ParseTrakteerWeb(url);
        requiredNodes.RawText.Should().BeNullOrEmpty();
    }

    [Theory]
    [InlineData("ca9c28da-87c0-5d32-9b6f-a220d3d36dfd")]
    [InlineData("https://trakteer.id/payment-status/ca9c28da-87c0-5d32-9b6f-a220d3d36dfd")]
    public async Task TrakteerApiTest(string url)
    {
        var trakteerApi = await mirrorPaymentService.GetTrakteerApi(url);

        trakteerApi.OrderId.Should().NotBeNullOrEmpty();
        trakteerApi.CendolCount.Should().BeGreaterThan(0);
        trakteerApi.AdminFees.Should().BeGreaterThan(0);
        trakteerApi.Total.Should().BeGreaterThan(0);
        trakteerApi.OrderDate.Should().BeMoreThan(default);
        trakteerApi.PaymentMethod.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("65190576-e653-47d2-b472-9a367a54ed23")]
    [InlineData("https://saweria.co/receipt/65190576-e653-47d2-b472-9a367a54ed23")]
    public async Task SaweriaApiTest(string url)
    {
        var saweriaApi = await mirrorPaymentService.GetSaweriaApi(url);

        saweriaApi.OrderId.Should().NotBeNullOrEmpty();
        saweriaApi.CendolCount.Should().BeGreaterThan(0);
        saweriaApi.Total.Should().BeGreaterThan(0);
        saweriaApi.OrderDate.Should().BeMoreThan(default);
    }

    [Theory()]
    [InlineData("65190576-e653-47d2-b472-9a367a54ed23")]
    [InlineData("https://saweria.co/receipt/65190576-e653-47d2-b472-9a367a54ed23")]
    public async Task SaweriaParserTest(string url)
    {
        var donationParsedDto = await mirrorPaymentService.ParseSaweriaWeb(url);

        donationParsedDto.IsValid.Should().BeTrue();
        donationParsedDto.CendolCount.Should().BeGreaterThan(0);
        donationParsedDto.OrderDate.Should().BeMoreThan(default);
        donationParsedDto.OrderId.Should().NotBeNullOrEmpty();
    }
}
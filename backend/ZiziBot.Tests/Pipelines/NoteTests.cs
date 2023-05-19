using FluentAssertions;
using Xunit;

namespace ZiziBot.Tests.Pipelines;

public class NoteTests
{
    private readonly MediatorService _mediatorService;
    private readonly AppSettingRepository _appSettingRepository;

    public NoteTests(MediatorService mediatorService, AppSettingRepository appSettingRepository)
    {
        _mediatorService = mediatorService;
        _appSettingRepository = appSettingRepository;
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CreateNoteTest(bool refreshNote)
    {
        var botMain = await _appSettingRepository.GetBotMain();

        var result = await _mediatorService.EnqueueAsync(new CreateNoteBotRequest()
        {
            BotToken = botMain.Token,
            Message = SampleMessages.CommonMessage,
            ReplyMessage = true,
            Query = "sample_query",
            Content = "lorem ipsum dolor",
            RawButton = "example|https://azhe.my.id",
            FileId = "AgACAgUAAx0CU7hehgABBIhxZFYPJ554nuA85DnmWX9_B33eTx8AAv-yMRuEgPFV7bW8s4uaTKABAAMCAAN5AAMvBA",
            DataType = (int)CommonMediaType.Photo,
            RefreshNote = refreshNote
        });

        result.ResponseSource.Should().Be(ResponseSource.Bot);
    }
}
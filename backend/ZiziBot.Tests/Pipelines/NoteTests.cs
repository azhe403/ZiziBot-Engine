using FluentAssertions;
using Xunit;

namespace ZiziBot.Tests.Pipelines;

public class NoteTests
{
    private readonly MediatorService _mediatorService;
    private readonly AppSettingRepository _appSettingRepository;
    private readonly MongoDbContextBase _mongoDbContext;

    public NoteTests(MediatorService mediatorService, AppSettingRepository appSettingRepository, MongoDbContextBase mongoDbContext)
    {
        _mediatorService = mediatorService;
        _appSettingRepository = appSettingRepository;
        _mongoDbContext = mongoDbContext;
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
            Query = "sample_query",
            Content = "lorem ipsum dolor",
            RawButton = "example|https://azhe.my.id",
            FileId = "AgACAgUAAx0CU7hehgABBIhxZFYPJ554nuA85DnmWX9_B33eTx8AAv-yMRuEgPFV7bW8s4uaTKABAAMCAAN5AAMvBA",
            DataType = (int)CommonMediaType.Photo,
            RefreshNote = refreshNote
        });

        result.ResponseSource.Should().Be(ResponseSource.Bot);
    }

    [Theory]
    [InlineData("ini-note-ngab")]
    public async Task DeleteNoteTest(string note)
    {
        // Arrange
        _mongoDbContext.Note.Add(new NoteEntity()
        {
            ChatId = SampleMessages.CommonMessage.Chat.Id,
            Query = note,
            Status = (int)EventStatus.Complete
        });

        await _mongoDbContext.SaveChangesAsync();


        // Act
        var botMain = await _appSettingRepository.GetBotMain();

        await _mediatorService.EnqueueAsync(new DeleteNoteRequest()
        {
            BotToken = botMain.Token,
            Message = SampleMessages.CommonMessage,
            Note = note
        });
    }

    [Theory]
    [InlineData("ini-note-ngab")]
    public async Task DeleteNoteAlreadyDeletedTest(string note)
    {
        var botMain = await _appSettingRepository.GetBotMain();

        await _mediatorService.EnqueueAsync(new DeleteNoteRequest()
        {
            BotToken = botMain.Token,
            Message = SampleMessages.CommonMessage,
            // ReplyMessage = true,
            Note = note
        });
    }
}
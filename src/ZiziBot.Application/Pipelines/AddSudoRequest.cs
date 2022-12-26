using JetBrains.Annotations;
using MediatR;

namespace ZiziBot.Application.Pipelines;

public class AddSudoRequestModel : RequestBase
{
}

[UsedImplicitly]
public class AddSudoRequestHandler : IRequestHandler<AddSudoRequestModel, ResponseBase>
{
    private readonly SudoService _sudoService;

    public AddSudoRequestHandler(AppSettingsDbContext appSettingsDbContext, SudoService sudoService)
    {
        _sudoService = sudoService;
    }

    public async Task<ResponseBase> Handle(AddSudoRequestModel request, CancellationToken cancellationToken)
    {
        ResponseBase responseBase = new(request);

        await responseBase.SendMessageText("Adding sudo user...");

        var serviceResult = await _sudoService.SaveSudo(new Sudoer()
        {
            UserId = request.UserId,
            PromotedBy = request.UserId,
            PromotedFrom = request.ChatId.Identifier!.Value,
            Status = (int)EventStatus.Complete
        });

        await responseBase.EditMessageText(serviceResult.Message);

        return responseBase.Complete();
    }
}
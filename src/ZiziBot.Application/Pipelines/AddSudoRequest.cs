using JetBrains.Annotations;
using MediatR;

namespace ZiziBot.Application.Pipelines;

public class AddSudoRequestModel : RequestBase
{
}

[UsedImplicitly]
public class AddSudoRequestHandler : IRequestHandler<AddSudoRequestModel, ResponseBase>
{
    private readonly AppSettingsDbContext _appSettingsDbContext;

    public AddSudoRequestHandler(AppSettingsDbContext appSettingsDbContext)
    {
        _appSettingsDbContext = appSettingsDbContext;
    }

    public async Task<ResponseBase> Handle(AddSudoRequestModel request, CancellationToken cancellationToken)
    {
        ResponseBase responseBase = new(request);

        await responseBase.SendMessageText("Adding sudo user...");

        var sudo = _appSettingsDbContext.Sudoers.FirstOrDefault(sudoer => sudoer.UserId == request.UserId);

        if (sudo!= null)
        {
            await responseBase.EditMessageText("User is already a sudoer.");
            return responseBase;
        }

        _appSettingsDbContext.Sudoers.Add(new Sudoer()
        {
            UserId = request.UserId,
            PromotedBy = request.UserId,
            PromotedFrom = request.ChatId.Identifier!.Value,
            Status = (int)EventStatus.Complete
        });

        await _appSettingsDbContext.SaveChangesAsync(cancellationToken);

        await responseBase.EditMessageText("Sudoer added successfully");

        return responseBase.Complete();
    }
}
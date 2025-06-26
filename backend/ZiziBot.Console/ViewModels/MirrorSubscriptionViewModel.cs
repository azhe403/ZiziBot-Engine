using Microsoft.EntityFrameworkCore;
using ZiziBot.Common.Enums;
using ZiziBot.Common.Types;
using ZiziBot.Database;
using ZiziBot.Database.MongoDb.Entities;

namespace ZiziBot.Console.ViewModels;

public class MirrorSubscriptionViewModel(DataFacade dataFacade) : ReactiveObject, IActivatableViewModel
{
    public ViewModelActivator Activator { get; }
    public LoadingConfiguration Loading { get; set; } = new();

    [Reactive]
    public List<MirrorUserEntity>? MirrorUsers { get; set; }

    public List<MirrorApprovalEntity>? MirrorApprovals { get; set; }

    public async Task LoadData()
    {
        await FindMirrorUsers();
        await FindMirrorApprovals();
    }

    public async Task FindMirrorUsers()
    {
        Loading.IsLoading = true;
        Loading.TotalSteps = 2;
        Loading.CurrentStep++;
        Loading.Title = "Loading Mirror Users";

        MirrorUsers = await dataFacade.MongoDb.MirrorUser
            .Where(x => x.Status == EventStatus.Complete)
            .OrderByDescending(o => o.UpdatedDate)
            .ToListAsync();

        Loading.CurrentStep++;
        Loading.IsLoading = false;
    }

    public async Task FindMirrorApprovals()
    {
        Loading.IsLoading = true;
        Loading.TotalSteps = 2;
        Loading.CurrentStep++;
        Loading.Title = "Loading Mirror Approvals";

        MirrorApprovals = await dataFacade.MongoDb.MirrorApproval
            .Where(x => x.Status == EventStatus.Complete)
            .OrderByDescending(o => o.UpdatedDate)
            .ToListAsync();

        Loading.CurrentStep++;
        Loading.IsLoading = false;
    }
}
using MongoFramework.Linq;
using ZiziBot.Application.Facades;
using ZiziBot.Contracts.Enums;
using ZiziBot.DataSource.MongoDb.Entities;
using ZiziBot.Types.Types;

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

        MirrorUsers = await dataFacade.MongoDb.MirrorUsers
            .Where(x => x.Status == (int)EventStatus.Complete)
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
            .Where(x => x.Status == (int)EventStatus.Complete)
            .OrderByDescending(o => o.UpdatedDate)
            .ToListAsync();

        Loading.CurrentStep++;
        Loading.IsLoading = false;
    }
}
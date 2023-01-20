using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.RestApis.MirrorUser;

public class GetMirrorUsersRequestDto : IRequest<List<MirrorUserEntity>>
{
}

public class GetMirrorUsersRequestHandler : IRequestHandler<GetMirrorUsersRequestDto, List<MirrorUserEntity>>
{
	private readonly MirrorDbContext _mirrorDbContext;

	public GetMirrorUsersRequestHandler(MirrorDbContext mirrorDbContext)
	{
		_mirrorDbContext = mirrorDbContext;
	}

    public async Task<List<MirrorUserEntity>> Handle(GetMirrorUsersRequestDto request, CancellationToken cancellationToken)
	{
		var user = await _mirrorDbContext.MirrorUsers
			.Where(mirrorUser => mirrorUser.Status == (int) EventStatus.Complete)
			.ToListAsync(cancellationToken: cancellationToken);

		return user;
	}
}
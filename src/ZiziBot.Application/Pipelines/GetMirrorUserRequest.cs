using MediatR;
using MongoFramework.Linq;

namespace ZiziBot.Application.Pipelines;

public class GetMirrorUsersRequestDto : IRequest<List<MirrorUser>>
{
}

public class GetMirrorUsersRequestHandler : IRequestHandler<GetMirrorUsersRequestDto, List<MirrorUser>>
{
	private readonly MirrorDbContext _mirrorDbContext;

	public GetMirrorUsersRequestHandler(MirrorDbContext mirrorDbContext)
	{
		_mirrorDbContext = mirrorDbContext;
	}

	public async Task<List<MirrorUser>> Handle(GetMirrorUsersRequestDto request, CancellationToken cancellationToken)
	{
		var user = await _mirrorDbContext.MirrorUsers
			.ToListAsync(cancellationToken: cancellationToken);

		return user;
	}
}
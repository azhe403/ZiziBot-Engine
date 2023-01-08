using MediatR;
using MongoFramework.Linq;

namespace ZiziBot.Application.Pipelines;

public class GetMirrorUserByUserIdRequestDto : IRequest<MirrorUser>
{
	public long UserId { get; set; }
}

public class GetMirrorUserByUserIdRequestHandler : IRequestHandler<GetMirrorUserByUserIdRequestDto, MirrorUser>
{
	private readonly MirrorDbContext _mirrorDbContext;

	public GetMirrorUserByUserIdRequestHandler(MirrorDbContext mirrorDbContext)
	{
		_mirrorDbContext = mirrorDbContext;
	}

	public async Task<MirrorUser> Handle(GetMirrorUserByUserIdRequestDto request, CancellationToken cancellationToken)
	{
		var user = await _mirrorDbContext.MirrorUsers
			.Where(x => x.UserId == request.UserId)
			.FirstOrDefaultAsync(cancellationToken: cancellationToken);

		return user;
	}
}
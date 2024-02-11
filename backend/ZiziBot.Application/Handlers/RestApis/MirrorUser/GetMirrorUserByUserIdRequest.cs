namespace ZiziBot.Application.Handlers.RestApis.MirrorUser;

public class GetMirrorUserByUserIdRequestDto : ApiRequestBase<GetMirrorUserDto>
{
    public long UserId { get; set; }
}

public class GetMirrorUserDto
{
    public long UserId { get; set; }
    public bool HasSubscription { get; set; }
    public DateTime? SubscriptionUntil { get; set; }
    public DateTime? MemberSince { get; set; }
}

public class GetMirrorUserByUserIdRequestHandler : IApiRequestHandler<GetMirrorUserByUserIdRequestDto, GetMirrorUserDto>
{
    private readonly MongoDbContextBase _mongoDbContext;
    private readonly MirrorUserRepository _mirrorUserRepository;
    private readonly ApiResponseBase<GetMirrorUserDto> _response = new();

    public GetMirrorUserByUserIdRequestHandler(MongoDbContextBase mongoDbContext, MirrorUserRepository mirrorUserRepository)
    {
        _mongoDbContext = mongoDbContext;
        _mirrorUserRepository = mirrorUserRepository;
    }

    public async Task<ApiResponseBase<GetMirrorUserDto>> Handle(GetMirrorUserByUserIdRequestDto request, CancellationToken cancellationToken)
    {
        var user = await _mirrorUserRepository.GetByUserId(request.UserId);

        if (user == null)
        {
            return _response.NotFound("Mirror User not found");
        }

        return _response.Success("Mirror User found", new GetMirrorUserDto
        {
            UserId = request.UserId,
            HasSubscription = user.ExpireDate > DateTime.UtcNow.AddHours(Env.DEFAULT_TIMEZONE),
            SubscriptionUntil = user.ExpireDate.AddHours(Env.DEFAULT_TIMEZONE),
            MemberSince = user.CreatedDate.AddHours(Env.DEFAULT_TIMEZONE)
        });
    }
}
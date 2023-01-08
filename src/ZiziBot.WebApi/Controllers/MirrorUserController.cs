using Microsoft.AspNetCore.Mvc;
using MongoFramework.Linq;

namespace ZiziBot.WebApi.Controllers
{
    [ApiController]
	[Route("mirror-user")]
    public class MirrorUserController : ApiControllerBase
    {
        private readonly MirrorDbContext _mirrorDbContext;

        public MirrorUserController(MirrorDbContext mirrorDbContext)
        {
            _mirrorDbContext = mirrorDbContext;
        }

        [HttpGet()]
        public async Task<List<MirrorUser>> GetUsers([FromQuery] long userId)
        {
            var user = await _mirrorDbContext.MirrorUsers
                .Where(user => user.UserId == userId)
                .ToListAsync();


            return user;
        }

        [HttpPost()]
        public async Task<bool> PostUserMirror([FromBody] PostMirrorUserRequestDto requestDto)
        {
            var result = await Mediator.Send(requestDto);

            return result;
        }
    }
}
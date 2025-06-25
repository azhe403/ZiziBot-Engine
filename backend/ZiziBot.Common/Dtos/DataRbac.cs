using ZiziBot.Common.Enums;

namespace ZiziBot.Common.Dtos;

public class DataRbac
{
    public string? TransactionId { get; set; }
    public string? AccessToken { get; set; }
    public long UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserPhotoUrl { get; set; }
    public string? UserFirstName { get; set; }
    public string? UserLastName { get; set; }
    public List<RoleLevel> UserRoles { get; set; } = [];
}
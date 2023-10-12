namespace ZiziBot.DataSource.MongoDb.Entities;

[Table("FeatureRoleFlag")]
public class FeatureRoleFlagEntity : EntityBase
{
    public int RoleFlagId { get; set; }
    public int RoleId { get; set; }
    public int FlagId { get; set; }
}
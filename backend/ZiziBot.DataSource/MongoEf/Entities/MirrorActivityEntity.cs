﻿namespace ZiziBot.DataSource.MongoEf.Entities;

[Table("MirrorActivity")]
public class MirrorActivityEntity : EntityBase
{
    public long UserId { get; set; }
    public int ActivityTypeId { get; set; }
    public string ActivityName { get; set; }
    public string Url { get; set; }
}
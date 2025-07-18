﻿using MongoDB.EntityFrameworkCore;

namespace ZiziBot.Database.MongoDb.Entities;

[Collection("FeatureRollout")]
public class FeatureRolloutEntity : EntityBase
{
    public required string FeatureName { get; set; }
    public FeatureRolloutCategory RolloutCategory { get; set; }
}
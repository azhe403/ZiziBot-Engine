﻿namespace ZiziBot.Common.Types;

/*
 * Reference: https://github.com/TAGC/dotnet-setversion/blob/develop/src/dotnet-setversion/VersionModel.cs
 * Copyright (c) ThymineC
 */

public class VersionModel
{
    public VersionModelDetail Version { get; set; }

    public override string ToString()
    {
        if (Version != null)
        {
            return $"{Version.Major}.{Version.Minor}.{Version.Patch}";
        }

        return string.Empty;
    }
}

public class VersionModelDetail
{
    public int Major { get; set; }
    public int Minor { get; set; }
    public int Patch { get; set; }
}
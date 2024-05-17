﻿namespace TournamentTracker.BusinessLayer.Settings;

public class AppSettings
{
    public string ApplicationName { get; init; }

    public string ApplicationDescription { get; init; }

    public int CommandTimeout { get; init; }

    public string ContainerName { get; init; }

    public int MaxRetryCount { get; init; }

    public TimeSpan MaxRetryDelay { get; init; }

    public string StorageFolder { get; init; }

    public string[] SupportedCultures { get; init; }
}
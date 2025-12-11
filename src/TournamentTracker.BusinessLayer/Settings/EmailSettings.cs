using System;
using System.Collections.Generic;
using System.Text;

namespace TournamentTracker.BusinessLayer.Settings;

public class EmailSettings
{
    public string Host { get; init; } = null!;

    public int Port { get; init; }

    public bool UseSsl { get; init; }

    public string? UserName { get; init; }

    public string? Password { get; init; }

    public bool IgnoreServerCertificateErrors { get; init; }
}
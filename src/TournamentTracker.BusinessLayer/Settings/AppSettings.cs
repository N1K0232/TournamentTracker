namespace TournamentTracker.BusinessLayer.Settings;

public class AppSettings
{
    public string ApplicationName { get; init; } = "Tournament Tracker";

    public string ApplicationDescription { get; init; } = "A C# web application to handle tournament subscription and tournament matches";

    public string[] SupportedCultures { get; init; } = [];
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleTransit;
using TournamentTracker.BusinessLayer.Clients.Interfaces;
using TournamentTracker.BusinessLayer.Resources;
using TournamentTracker.BusinessLayer.Settings;
using TournamentTracker.DataAccessLayer;
using TournamentTracker.Shared.Models;
using TournamentTracker.Shared.Models.Notifications;
using Entities = TournamentTracker.DataAccessLayer.Entities;

namespace TournamentTracker.BusinessLayer.Notifications;

public class PersonEmailNotificationHandler(IDataContext dataContext, IEmailClient emailClient, IOptions<AppSettings> appSettingsOptions) : INotificationHandler<PersonEmailNotificationMessage>
{
    private readonly AppSettings appSettings = appSettingsOptions.Value;

    public async Task HandleAsync(PersonEmailNotificationMessage message, CancellationToken cancellationToken)
    {
        var team = await dataContext.GetData<Entities.Team>()
            .Include(t => t.Tournament)
            .FirstAsync(t => t.Id == message.TeamId, cancellationToken);

        var emailMessage = new EmailMessage
        {
            SenderEmail = appSettings.SenderEmail,
            SenderName = appSettings.SenderName,
            To = [message.EmailAddress],
            Subject = "Team invite",
            TextContent = string.Format(Messages.TeamInvite, team.Name, team.Tournament.Name, team.Tournament.StartsAt)
        };

        await emailClient.SendAsync(emailMessage, cancellationToken);
    }
}
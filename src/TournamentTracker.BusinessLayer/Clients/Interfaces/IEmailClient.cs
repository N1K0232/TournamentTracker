using TournamentTracker.Shared.Models;

namespace TournamentTracker.BusinessLayer.Clients.Interfaces;

public interface IEmailClient
{
    Task SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default);
}
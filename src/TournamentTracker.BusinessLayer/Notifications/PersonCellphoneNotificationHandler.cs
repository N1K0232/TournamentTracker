using SimpleTransit;
using TournamentTracker.Shared.Models.Notifications;

namespace TournamentTracker.BusinessLayer.Notifications;

public class PersonCellphoneNotificationHandler : INotificationHandler<PersonCellphoneNotificationMessage>
{
    public Task HandleAsync(PersonCellphoneNotificationMessage message, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
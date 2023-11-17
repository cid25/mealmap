using Mealmap.Api.Shared;

namespace Mealmap.Api.UnitTests.Shared;

public class CommandNotificationTests
{
    [Fact]
    public void Success_False_WhenErrors()
    {
        CommandNotification<object> notification = new();

        notification.Errors.Add(new CommandError(CommandErrorCodes.NotFound, ""));

        notification.Succeeded.Should().BeFalse();
    }
}

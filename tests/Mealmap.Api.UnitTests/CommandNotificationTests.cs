namespace Mealmap.Api.UnitTests;

public class CommandNotificationTests
{
    [Fact]
    public void Success_False_WhenErrors()
    {
        CommandNotification<Object> notification = new();

        notification.Errors.Add(new CommandError(CommandErrorCodes.NotFound, ""));

        notification.Success.Should().BeFalse();
    }
}

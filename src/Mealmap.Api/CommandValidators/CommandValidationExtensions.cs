namespace Mealmap.Api.CommandValidators;

public static class CommandValidationExtensions
{
    public static List<CommandError> PotentiallyWithErrorsFrom(this List<CommandError> errors,
        IEnumerable<CommandError> furtherErrors)
    {
        errors.AddRange(furtherErrors);
        return errors;
    }

    public static List<CommandError> PotentiallyWithErrorFrom(this List<CommandError> errors,
    CommandError? error)
    {
        if (error != null)
            errors.Add(error);
        return errors;
    }
}

namespace Mealmap.Domain.Common.Validation;

public class DomainValidationResult
{
    private readonly List<string> _errors = [];

    public IReadOnlyCollection<string> Errors
    {
        get => _errors.AsReadOnly();
    }

    public bool IsValid { get => _errors.Count == 0; }

    public void AddError(string message)
        => _errors.Add(message);
}

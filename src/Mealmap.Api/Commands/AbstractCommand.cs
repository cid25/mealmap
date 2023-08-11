namespace Mealmap.Api.Commands;

public abstract class AbstractCommand<TDataTransferObject>
{
    public TDataTransferObject Dto { get; protected set; }

    public AbstractCommand(TDataTransferObject dto)
    {
        Dto = dto;
    }
}

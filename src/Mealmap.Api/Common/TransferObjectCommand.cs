namespace Mealmap.Api.Shared;

public abstract class TransferObjectCommand<TDataTransferObject>
{
    public TDataTransferObject Dto { get; protected set; }

    public TransferObjectCommand(TDataTransferObject dto)
    {
        Dto = dto;
    }
}

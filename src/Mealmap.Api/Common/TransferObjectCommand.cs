namespace Mealmap.Api.Common;

public abstract class TransferObjectCommand<TDataTransferObject>(TDataTransferObject dto)
{
    public TDataTransferObject Dto { get; protected set; } = dto;
}

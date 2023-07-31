namespace Mealmap.Api.InputMappers
{
    public interface IInputHandler<TEntity, TDataTransferObject>
    {
        public TEntity FromDataTransferObject(TDataTransferObject dto);
    }
}

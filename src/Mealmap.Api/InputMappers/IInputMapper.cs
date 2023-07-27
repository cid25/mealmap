namespace Mealmap.Api.InputMappers
{
    public interface IInputMapper<TEntity, TDataTransferObject>
    {
        public TEntity FromDataTransferObject(TDataTransferObject dto);
    }
}

namespace Mealmap.Api.Common;

public interface IOutputMapper<TDataTransferObject, TEntity>
{
    TDataTransferObject FromEntity(TEntity entity);

    IEnumerable<TDataTransferObject> FromEntities(IEnumerable<TEntity> entities);
}

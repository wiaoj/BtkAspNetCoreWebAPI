using Entities.Models;

namespace Services.Contracts;
public interface IDataShaper<Type> {
    IEnumerable<ShapedEntity> ShapeData(IEnumerable<Type> entities, String fieldsString);
    ShapedEntity ShapeData(Type entity, String fieldsString);
}
using Entities.Models;
using Services.Contracts;
using System.Reflection;

namespace Services;
public class DataShaper<Type> : IDataShaper<Type> where Type : class {
    public PropertyInfo[] Properties { get; set; }

    public DataShaper() {
        this.Properties = typeof(Type).GetProperties(BindingFlags.Public | BindingFlags.Instance);
    }

    public IEnumerable<ShapedEntity> ShapeData(IEnumerable<Type> entities, String fieldsString) {
        IEnumerable<PropertyInfo> requiredFields = this.GetRequiredProperties(fieldsString);
        return this.FetchData(entities, requiredFields);
    }

    public ShapedEntity ShapeData(Type entity, String fieldsString) {
        IEnumerable<PropertyInfo> requiredProperties = this.GetRequiredProperties(fieldsString);
        return this.FetchDataForEntity(entity, requiredProperties);
    }

    private IEnumerable<PropertyInfo> GetRequiredProperties(String fieldsString) {
        List<PropertyInfo> requiredFields = new();
        if(String.IsNullOrWhiteSpace(fieldsString) is false) {
            String[] fields = fieldsString.Split(',', StringSplitOptions.RemoveEmptyEntries);

            foreach(String field in fields) {
                PropertyInfo? property = this.Properties
                    .FirstOrDefault(propertyInfo => propertyInfo.Name.Equals(field.Trim(), StringComparison.InvariantCultureIgnoreCase));

                if(property is null)
                    continue;

                requiredFields.Add(property);
            }

        } else {
            requiredFields = this.Properties.ToList();
        }

        return requiredFields;
    }

    private ShapedEntity FetchDataForEntity(Type entity, IEnumerable<PropertyInfo> requiredProperties) {
        ShapedEntity shapedObject = new();

        foreach(PropertyInfo property in requiredProperties) {
            Object? objectPropertyValue = property.GetValue(entity);
            shapedObject.Entity.TryAdd(property.Name, objectPropertyValue);
        }

        PropertyInfo? objectProperty = entity.GetType().GetProperty("Id");
        shapedObject.Id = Guid.Parse($"{objectProperty.GetValue(entity)}");

        return shapedObject;
    }

    private IEnumerable<ShapedEntity> FetchData(IEnumerable<Type> entities, IEnumerable<PropertyInfo> requiredProperties) {
        List<ShapedEntity> shapedData = new();

        foreach(Type entity in entities) {
            ShapedEntity shapedObject = this.FetchDataForEntity(entity, requiredProperties);
            shapedData.Add(shapedObject);
        }

        return shapedData;
    }
}
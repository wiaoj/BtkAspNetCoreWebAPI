using System.Reflection;
using System.Text;

namespace Repositories.EFCore.Extensions;
public static class OrderQueryBuilder {
    public static String CreateOrderQuery<Type>(String orderByQueryString) {
        String[] orderParams = orderByQueryString.Trim().Split(',');

        PropertyInfo[] propertyInfos = typeof(Type).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        StringBuilder orderQueryBuilder = new();

        foreach(String param in orderParams) {
            if(String.IsNullOrWhiteSpace(param))
                continue;

            String propertyFromQueryName = param.Split(' ')[0];

            PropertyInfo? objectProperty = propertyInfos
                .FirstOrDefault(pi => pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));

            if(objectProperty is null)
                continue;

            String direction = param.EndsWith(" desc") ? "descending" : "ascending";

            orderQueryBuilder.Append($"{objectProperty.Name.ToString()}  {direction},");
        }

        String orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');

        return orderQuery;
    }
}
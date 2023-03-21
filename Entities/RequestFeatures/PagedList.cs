namespace Entities.RequestFeatures;
public class PagedList<Type> : List<Type> {
    public MetaData MetaData { get; init; }
    public PagedList(List<Type> items, Int32 count, Int32 pageNumber, Int32 pageSize) {
        this.MetaData = new MetaData(
            currentPage: pageNumber,
            totalPage: (Int32)Math.Ceiling(count / (Double)pageSize),
            pageSize: pageSize,
            totalCount: count);
        this.AddRange(items);
    }

    public static PagedList<Type> ToPagedList(IEnumerable<Type> source, Int32 pageNumber, Int32 pageSize) {
        Int32 count = source.Count();
        List<Type> items = source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedList<Type>(items, count, pageNumber, pageSize);
    }
}
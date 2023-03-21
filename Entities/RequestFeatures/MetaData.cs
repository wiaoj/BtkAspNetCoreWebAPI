namespace Entities.RequestFeatures;
public class MetaData {
    public Int32 CurrentPage { get; init; }
    public Int32 TotalPage { get; init; }
    public Int32 PageSize { get; init; }
    public Int32 TotalCount { get; init; }

    public Boolean HasPrevious => this.CurrentPage > 1;
    public Boolean HasPage => this.CurrentPage < this.TotalPage;

    public MetaData(Int32 currentPage, Int32 totalPage, Int32 pageSize, Int32 totalCount) {
        this.CurrentPage = currentPage;
        this.TotalPage = totalPage;
        this.PageSize = pageSize;
        this.TotalCount = totalCount;
    }

}
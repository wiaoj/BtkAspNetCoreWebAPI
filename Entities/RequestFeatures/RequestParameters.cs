namespace Entities.RequestFeatures;
public abstract class RequestParameters {
    private const Int32 maxPageSize = 50;

    // Auto-implemented property 
    public Int32 PageNumber { get; set; }

    // Full-property
    private Int32 pageSize;

    public Int32 PageSize {
        get => this.pageSize;
        set => this.pageSize = value > maxPageSize ? maxPageSize : value;
    }

    public String? OrderBy { get; set; }
    public String? Fields { get; set; }
}
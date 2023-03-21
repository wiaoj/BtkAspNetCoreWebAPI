namespace Entities.RequestFeatures;
public class BookParameters : RequestParameters {
    public UInt32 MinPrice { get; set; }
    public UInt32 MaxPrice { get; set; } = UInt32.MaxValue;
    public Boolean ValidPriceRange => this.MaxPrice > this.MinPrice;

    public String? SearchTerm { get; set; }

    public BookParameters() {
        this.OrderBy = "id";
    }
}
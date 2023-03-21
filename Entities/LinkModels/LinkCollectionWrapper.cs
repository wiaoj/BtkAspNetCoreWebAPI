namespace Entities.LinkModels;
public class LinkCollectionWrapper<T> : LinkResourceBase {
    public List<T> Value { get; set; }

    public LinkCollectionWrapper() : this(new List<T>()) { }

    public LinkCollectionWrapper(List<T> value) {
        this.Value = value;
    }
}
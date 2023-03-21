namespace Entities.LinkModels;
public class LinkResourceBase {
    public List<Link> Links { get; set; }

    public LinkResourceBase() {
        this.Links = new List<Link>();
    }
}
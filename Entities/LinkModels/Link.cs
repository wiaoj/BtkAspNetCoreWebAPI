namespace Entities.LinkModels;
public sealed class Link {
    public String? Href { get; set; }
    public String? Rel { get; set; }
    public String? Method { get; set; }

    public Link() { }

    public Link(String? href, String? rel, String? method) {
        this.Href = href;
        this.Rel = rel;
        this.Method = method;
    }
}
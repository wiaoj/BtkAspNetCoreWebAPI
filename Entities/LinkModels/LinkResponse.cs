using Entities.Models;


namespace Entities.LinkModels;
public class LinkResponse {
    public Boolean HasLinks { get; set; }
    public List<Entity> ShapedEntities { get; set; }
    public LinkCollectionWrapper<Entity> LinkedEntities { get; set; }

    public LinkResponse() {
        this.ShapedEntities = new List<Entity>();
        this.LinkedEntities = new LinkCollectionWrapper<Entity>();
    }
}
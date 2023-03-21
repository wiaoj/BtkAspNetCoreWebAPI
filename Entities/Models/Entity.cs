using Entities.LinkModels;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Entities.Models;
public class Entity : DynamicObject, IXmlSerializable, IDictionary<String, Object> {
    private readonly String root = nameof(Entity);
    private readonly IDictionary<String, Object> expando;

    public Entity() {
        this.expando = new ExpandoObject();
    }

    public Object this[String key] {
        get => this.expando[key];
        set => this.expando[key] = value;
    }

    public ICollection<String> Keys => this.expando.Keys;

    public ICollection<Object> Values => this.expando.Values;

    public Int32 Count => this.expando.Count;

    public Boolean IsReadOnly => this.expando.IsReadOnly;

    public void Add(String key, Object value) {
        this.expando.Add(key, value);
    }

    public void Add(KeyValuePair<String, Object> item) {
        this.expando.Add(item);
    }

    public void Clear() {
        this.expando.Clear();
    }

    public Boolean Contains(KeyValuePair<String, Object> item) {
        return this.expando.Contains(item);
    }

    public Boolean ContainsKey(String key) {
        return this.expando.ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<String, Object>[] array, Int32 arrayIndex) {
        this.expando.CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<String, Object>> GetEnumerator() {
        return this.expando.GetEnumerator();
    }

    public XmlSchema? GetSchema() {
        throw new NotImplementedException();
    }

    public void ReadXml(XmlReader reader) {
        reader.ReadStartElement(this.root);

        while(reader.Name.Equals(this.root)) {
            String typeContent;
            Type? underlyingType;
            String name = reader.Name;

            reader.MoveToAttribute("type");
            typeContent = reader.ReadContentAsString();
            underlyingType = Type.GetType(typeContent);
            reader.MoveToContent();
            this.expando[name] = reader.ReadElementContentAs(underlyingType, null);
        }
    }

    public Boolean Remove(String key) {
        return this.expando.Remove(key);
    }

    public Boolean Remove(KeyValuePair<String, Object> item) {
        return this.expando.Remove(item);
    }

    public Boolean TryGetValue(String key, [MaybeNullWhen(false)] out Object value) {
        return this.expando.TryGetValue(key, out value);
    }

    public void WriteXml(XmlWriter writer) {
        foreach(String key in this.expando.Keys) {
            Object value = this.expando[key];
            this.WriteLinksToXml(key, value, writer);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return this.GetEnumerator();
    }

    private void WriteLinksToXml(String key, Object value, XmlWriter writer) {
        writer.WriteStartElement(key);

        if(value.GetType() == typeof(List<Link>)) {
            foreach(Link val in value as List<Link>) {
                writer.WriteStartElement(nameof(Link));
                this.WriteLinksToXml(nameof(val.Href), val.Href, writer);
                this.WriteLinksToXml(nameof(val.Method), val.Method, writer);
                this.WriteLinksToXml(nameof(val.Rel), val.Rel, writer);
                writer.WriteEndElement();
            }
        } else {
            writer.WriteString(value.ToString());
        }

        writer.WriteEndElement();
    }
}

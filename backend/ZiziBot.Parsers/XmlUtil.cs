using System.Xml.Linq;

namespace ZiziBot.Parsers;

public static class XmlUtil
{
    public static XElement GetOrCreateElement(this XContainer container, string name)
    {
        var element = container.Element(name);
        if (element != null) return element;

        element = new XElement(name);
        container.Add(element);
        return element;
    }
}
namespace  PlayerSpace;

public class AttributeDto
{
    public string Attribute { get; set; }   // save the enum name (ex: "Strength")
    public int AttributeLevel { get; set; } // only unique data
}
using Godot;
using System;

namespace PlayerSpace
{
    [GlobalClass]

    public partial class AttributeData : Resource
    {
        [Export] public AllEnums.Attributes attribute;
        [Export] public string attributeAbbreviation;
        [Export] public string attributeName;
        [Export] public string attributeDescription;
        [Export] public int attributeLevel;
    }
}
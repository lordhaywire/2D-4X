using Godot;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class FactionResourceData : Resource
    {
        [Export] public string goodName;
        [Export] public string description;
        [Export] public AllEnums.FactionResourceType resourceType;
        [Export] public int amount;
    }
}
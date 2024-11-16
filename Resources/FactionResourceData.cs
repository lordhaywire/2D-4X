using Godot;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class FactionResourceData : Resource, IGoodsData
    {
        [Export] public string GoodName { get; set; }
        [Export] public string description;
        [Export] public AllEnums.FactionResourceType resourceType;
        [Export] public int amount;
    }
}
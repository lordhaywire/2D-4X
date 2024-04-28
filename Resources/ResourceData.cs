using Godot;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class ResourceData : Resource
    {
        [Export] public string resourceName;
        [Export] public AllEnums.FactionResourceType resourceType;
        [Export] public bool perishable;

        public int amount;
        public int maxAmount;
    }
}
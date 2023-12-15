using Godot;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class ResourceData : Resource
    {
        [Export] public string resourceName;
        [Export] public AllEnums.ResourceType resourceType;
    }
}
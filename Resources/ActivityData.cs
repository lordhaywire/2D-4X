using Godot;

namespace PlayerSpace;

[GlobalClass]
public partial class ActivityData : Resource
{
    [Export] public string name;
    [Export] public string description;
    [Export] public string toolTip;
}
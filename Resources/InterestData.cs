using Godot;

namespace PlayerSpace;

[GlobalClass]
public partial class InterestData : Resource
{
    [Export] public AllEnums.InterestType interestType;
    [Export] public string interestName;
    [Export] public string interestDescription;
}
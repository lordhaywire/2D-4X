using Godot;

namespace PlayerSpace;

[GlobalClass]
public partial class PerkData : Resource
{
    [Export] public string perkName;
    [Export] public string perkDescription;
    [Export] public int perkBonus;
}
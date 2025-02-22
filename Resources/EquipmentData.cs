using Godot;

namespace PlayerSpace;

[GlobalClass]
public partial class EquipmentData : Resource
{
    [Export] public AllEnums.EquipmentType equipmentType;
    [Export] public int equipmentTier;
}
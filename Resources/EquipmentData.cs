using System;
using Godot;

namespace PlayerSpace;

[GlobalClass]
public partial class EquipmentData : Resource
{
    [Export] public AllEnums.InventorySlot inventorySlot;
    [Export] public int equipmentTier;
    [Export] public int equipmentBonus;

    // 🔹 Convert Resource → DTO
    public EquipmentDto ToDto()
    {
        return new EquipmentDto
        {
            InventorySlot = inventorySlot.ToString(), // store enum as string
            EquipmentTier = equipmentTier,
            EquipmentBonus = equipmentBonus
        };
    }

    // 🔹 Convert DTO → Resource
    public static EquipmentData FromDto(EquipmentDto dto)
    {
        return new EquipmentData
        {
            inventorySlot = Enum.Parse<AllEnums.InventorySlot>(dto.InventorySlot),
            equipmentTier = dto.EquipmentTier,
            equipmentBonus = dto.EquipmentBonus
        };
    }
}
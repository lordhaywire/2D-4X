using Godot;

namespace PlayerSpace;

public partial class InventoryVBoxContainer : VBoxContainer
{
    [Export] public CheckBox newestEquipment;
    [Export] public Label[] equipment;

    public void GenerateEquipment(PopulationData populationData)
    {
        for (int i = 1; i < equipment.Length; i++)
        {

        }
    }
}

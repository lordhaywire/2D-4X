using Godot;

namespace PlayerSpace;

public partial class InventoryVBoxContainer : VBoxContainer
{
    [Export] public CheckBox newestEquipment;
    [Export] public Label[] equipment;

    public void GenerateEquipment(PopulationData populationData)
    {
        if (populationData.isHero)
        {
            PopulationDescriptionControl.Instance.InventoryAndSubordinatesInventoryVBoxContainer.Show();

            for (int i = 0; i < equipment.Length; i++)
            {
                if (populationData.equipment[i] != null)
                {
                    equipment[i].Text = populationData.equipment[i].goodName;
                }
                else
                {
                    equipment[i].Text = $"{Tr("WORD_NONE")}";
                }
            }
        }
        else
        {
            PopulationDescriptionControl.Instance.InventoryAndSubordinatesInventoryVBoxContainer.Hide();

        }
    }
}
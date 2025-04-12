using System.Collections.Generic;
using System.Linq;
using Godot;

namespace PlayerSpace;

public partial class InventoryVBoxContainer : VBoxContainer
{
    [Export] public CheckBox newestEquipment;
    private readonly List<Label> equipment = [];

    public override void _Ready()
    {
        GetEquipmentLables();
    }

    private void GetEquipmentLables()
    {
        foreach (Node node in GetChildren().Skip(2))
        {
            if (node is Label)
            {
                equipment.Add(node as Label);
            }
        }
    }

    public void PopulateEquipment(PopulationData populationData)
    {
        if (populationData.isHero)
        {
            PopulationDescriptionControl.Instance.InventoryAndSubordinatesInventoryVBoxContainer.Show();

            for (int i = 0; i < equipment.Count; i++)
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

    private void NewestEquipmentCheckboxPressed()
    {
        PopulationData populationData = PopulationDescriptionControl.Instance.populationData;
        CountyData locationCountyData = Globals.Instance.GetCountyDataFromLocationId(populationData.location);

        GD.Print("Newest Equipment Checkbox has been pressed. " + newestEquipment.ButtonPressed);
        GD.Print("Token Movement - MoveToken: " + populationData.heroToken?.tokenMovement.MoveToken);
        if(populationData.heroToken?.tokenMovement.MoveToken == true)
        {
            populationData.useNewestEquipment = newestEquipment.ButtonPressed;
            return;
        }
        if (Globals.Instance.CheckIfPlayerFaction(locationCountyData.factionData) == true)
        {
            Quartermaster.EquipHeroes(populationData);
            PopulateEquipment(populationData);
        }
    }
}
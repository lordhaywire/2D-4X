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
        GetEquipmentLabels();
    }

    private void GetEquipmentLabels()
    {
        foreach (Node node in GetChildren().Skip(2))
        {
            if (node is Label label)
            {
                equipment.Add(label);
            }
        }
    }

    private void ChangeAllInventoryItemsToNone()
    {
        foreach (Label label in equipment)
        {
            label.Text = $"{Tr("WORD_NONE")}";
        }
    }
    
    public void PopulateHeroEquipment(PopulationData populationData)
    {
        ChangeAllInventoryItemsToNone();
        PopulationDescriptionControl.Instance.newestEquipmentCheckBox.Show();
        PopulationDescriptionControl.Instance.subordinatesVBoxContainer.Show();

        if (populationData.isHero || populationData.activity == AllEnums.Activities.Service)
        {
            PopulationDescriptionControl.Instance.inventoryAndSubordinatesInventoryVBoxContainer.Show();
            if (populationData.isHero || populationData.activity == AllEnums.Activities.Service)
            {
                for (int i = 0; i < equipment.Count; i++)
                {
                    equipment[i].Text = populationData.equipment[i] != null
                        ? populationData.equipment[i].goodName
                        : $"{Tr("WORD_NONE")}";
                }
            }

            if (populationData.activity == AllEnums.Activities.Service)
            {
                PopulationDescriptionControl.Instance.subordinatesVBoxContainer.Hide();
                PopulationDescriptionControl.Instance.newestEquipmentCheckBox.Hide();
            }
        }
        else
        {
            PopulationDescriptionControl.Instance.inventoryAndSubordinatesInventoryVBoxContainer.Hide();
        }
    }

    private void NewestEquipmentCheckboxPressed()
    {
        PopulationData populationData = PopulationDescriptionControl.Instance.populationData;
        CountyData locationCountyData = Globals.Instance.GetCountyDataFromLocationId(populationData.location);

        GD.Print("Newest Equipment Checkbox has been pressed. " + newestEquipment.ButtonPressed);
        GD.Print("Token Movement - MoveToken: " + populationData.heroToken?.tokenMovement.MoveToken);
        if (populationData.heroToken?.tokenMovement.MoveToken == true)
        {
            populationData.useNewestEquipment = newestEquipment.ButtonPressed;
            return;
        }

        if (Globals.Instance.CheckIfPlayerFaction(locationCountyData.factionData))
        {
            Quartermaster.EquipHeroesAndSubordinates(populationData);
            PopulateHeroEquipment(populationData);
        }
    }
}
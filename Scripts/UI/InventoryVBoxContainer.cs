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
                    // Convert int index 'i' to InventorySlot enum, skipping 'None' (which is 0)
                    // So 'i' = 0 corresponds to Reconnaissance (1), 'i' = 1 -> Offensive (2), etc.
                    AllEnums.InventorySlot slot = (AllEnums.InventorySlot)(i + 1);

                    // Get the GoodData for that slot, if any
                    GoodData goodData = null;
                    if (populationData.inventory != null && populationData.inventory.TryGetValue(slot, out GoodData value))
                    {
                        goodData = value;
                    }

                    equipment[i].Text = goodData != null
                        ? goodData.goodName
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
        FactionData factionData = SaveManager.Instance.saveGameData.ConvertFactionIdToFactionData(locationCountyData.factionId);

        GD.Print("Newest Equipment Checkbox has been pressed. " + newestEquipment.ButtonPressed);
        GD.Print("Token Movement - MoveToken: " + populationData.heroToken?.tokenMovement.MoveToken);
        if (populationData.heroToken?.tokenMovement.MoveToken == true)
        {
            populationData.useNewestEquipment = newestEquipment.ButtonPressed;
            return;
        }

        if (Globals.CheckIfPlayerFaction(factionData))
        {
            Quartermaster.EquipHeroesAndSubordinates(populationData);
            PopulateHeroEquipment(populationData);
        }
    }
}
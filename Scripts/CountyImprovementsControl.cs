using Godot;
using PlayerSpace;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace;

public partial class CountyImprovementsControl : Control
{
    public static CountyImprovementsControl Instance { get; private set; }

    [Export] private VBoxContainer possibleImprovementsScrollContainerParent;
    [Export] public VBoxContainer currentImprovementsScrollContainerParent;
    [Export] public ConfirmationDialog buildConfirmationDialog;

    [Export] private PackedScene countyImprovementButtonPackedScene;

    public override void _Ready()
    {
        Instance = this;
    }
    private void OnVisibilityChanged()
    {
        if (Visible == true)
        {
            Clock.Instance.PauseTime();
            CreateAllCountyImprovementButtons();
            PlayerControls.Instance.AdjustPlayerControls(false);
            CountyInfoControl.Instance.populationDescriptionControl.Hide();
            CountyInfoControl.Instance.populationListMarginContainer.Hide();
        }
        else
        {
            Clock.Instance.UnpauseTime();
            PlayerControls.Instance.AdjustPlayerControls(true);
        }
    }

    /// <summary>
    /// This goes through all of the county improvement lists and generates them for the player UI.
    /// </summary>
    public void CreateAllCountyImprovementButtons()
    {
        ClearImprovements();
        GD.PrintRich($"[rainbow]Count of county improvements: " + Globals.Instance.playerFactionData.allCountyImprovements.Count);

        CreateCountyImprovementButtons(Globals.Instance.playerFactionData.allCountyImprovements
            , possibleImprovementsScrollContainerParent);
        CreateCountyImprovementButtons(Globals.Instance.SelectedLeftClickCounty.countyData.underConstructionCountyImprovements
            , currentImprovementsScrollContainerParent);
        CreateCountyImprovementButtons(Globals.Instance.SelectedLeftClickCounty.countyData.completedCountyImprovements
            , currentImprovementsScrollContainerParent);
    }

    private void CreateCountyImprovementButtons(List<CountyImprovementData> listOfCountyImprovements, VBoxContainer parent)
    {
        GD.Print("List of County Improvements Count: " + listOfCountyImprovements.Count);
        foreach (CountyImprovementData countyImprovementData in listOfCountyImprovements)
        {
            CountryImprovementDescriptionButton countyImprovementButton = (CountryImprovementDescriptionButton)countyImprovementButtonPackedScene.Instantiate();
            // This needs to be above AddChild.
            countyImprovementButton.countyImprovementData = CountyImprovementData.NewCopy(countyImprovementData);
            parent.AddChild(countyImprovementButton);
        }
    }

    private void ClearImprovements()
    {
        foreach (Node node in possibleImprovementsScrollContainerParent.GetChildren().Skip(1))
        {
            node.QueueFree();
        }
        foreach (Node node in currentImprovementsScrollContainerParent.GetChildren().Skip(1))
        {
            node.QueueFree();
        }
    }
    private void CloseButton()
    {
        Hide();
    }
}
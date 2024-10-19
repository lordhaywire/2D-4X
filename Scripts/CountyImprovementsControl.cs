using Godot;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace;

public partial class CountyImprovementsControl : Control
{
    public static CountyImprovementsControl Instance { get; private set; }

    [Export] private VBoxContainer possibleImprovementsScrollContainerParent;
    [Export] public VBoxContainer currentImprovementsScrollContainerParent;
    [Export] public ConfirmationDialog buildConfirmationDialog;

    [Export] private PackedScene countyImprovementPanelContainerPackedScene;

    public override void _Ready()
    {
        Instance = this;
    }
    
    private void OnCountyImprovementControlVisibilityChanged()
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
        //GD.PrintRich($"[rainbow]Count of county improvements: " + Globals.Instance.playerFactionData.allCountyImprovements.Count);

        CreateCountyImprovementButtons(Globals.Instance.playerFactionData.allCountyImprovements
            , possibleImprovementsScrollContainerParent, true);
        CreateCountyImprovementButtons(Globals.Instance.SelectedLeftClickCounty.countyData.underConstructionCountyImprovements
            , currentImprovementsScrollContainerParent, false);
        CreateCountyImprovementButtons(Globals.Instance.SelectedLeftClickCounty.countyData.completedCountyImprovements
            , currentImprovementsScrollContainerParent, false);
    }

    /// <summary>
    /// createAllCountyImprovement Buttons calls this three times to create them all.
    /// </summary>
    /// <param name="listOfCountyImprovements"></param>
    /// <param name="parent"></param>
    private void CreateCountyImprovementButtons(List<CountyImprovementData> listOfCountyImprovements
        , VBoxContainer parent, bool allCountyImprovements)
    {
        GD.Print("List of County Improvements Count: " + listOfCountyImprovements.Count);
        listOfCountyImprovements.Sort((x, y) => string.Compare(x.improvementName, y.improvementName));

        foreach (CountyImprovementData countyImprovementData in listOfCountyImprovements)
        {
            CountryImprovementPanelContainer countyImprovementPanelContainer = (CountryImprovementPanelContainer)countyImprovementPanelContainerPackedScene.Instantiate();
            // This needs to be above AddChild.
            if (allCountyImprovements)
            {
                countyImprovementPanelContainer.countyImprovementData = CountyImprovementData.NewCopy(countyImprovementData);
            }
            else
            {
                countyImprovementPanelContainer.countyImprovementData = countyImprovementData;
            }
            parent.AddChild(countyImprovementPanelContainer);
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
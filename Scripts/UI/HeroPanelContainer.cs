using Godot;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace;

public partial class HeroPanelContainer : PanelContainer
{
    public PopulationData populationData;

    [Export] public TextureRect factionLeaderTextureRect;
    [Export] public TextureRect aideTextureRect;
    [Export] public TextureRect armyLeaderTextureRect;
    [Export] public Label heroNameLabel;
    [Export] public Button heroDescriptionButton;
    [Export] public CheckButton spawnHeroButton;
    [Export] public HBoxContainer primaryActivitiesHBoxContainer;
    [Export] public HBoxContainer secondaryActivitiesHBoxContainer;
    [Export] public HBoxContainer movementActivityHBoxContainer;
    public readonly List<CheckBox> heroCheckBoxesList = [];
    public readonly List<CheckBox> secondaryCheckBoxesList = [];

    public override void _Ready()
    {
        if (primaryActivitiesHBoxContainer == null)
        {
            return;
        }

        foreach (CheckBox checkBox in primaryActivitiesHBoxContainer.GetChildren().Cast<CheckBox>())
        {
            heroCheckBoxesList.Add(checkBox);
        }
        GetSecondaryCheckboxes();
        ConnectButtonSignals();
    }

    private void ConnectButtonSignals()
    {
        secondaryCheckBoxesList[0].Pressed += OnRecruitingCheckBoxPressed;
    }

    private void OnRecruitingCheckBoxPressed()
    {
        if (secondaryCheckBoxesList[0].ButtonPressed != true)
        {
            populationData.numberOfSubordinatesWanted = populationData.heroSubordinates.Count;
            secondaryCheckBoxesList[0].Disabled = true;
        }
    }

    public void HidePrimaryActivitiesHBoxContainer()
    {
        primaryActivitiesHBoxContainer.Hide();
    }

    public void HideSecondaryActivitiesHBoxContainer()
    {
        secondaryActivitiesHBoxContainer.Hide();
    }

    public void HideMovementActivityHBoxContainer()
    {
        movementActivityHBoxContainer.Hide();
    }

    public void ShowMovementActivityHBoxContainer()
    {
        Label movementLabel = (Label)movementActivityHBoxContainer.GetChild(0);
        CountyData currentLocationCountyData = Globals.Instance.GetCountyDataFromLocationID(populationData.location);
        CountyData destinationLocationCountyData = Globals.Instance.GetCountyDataFromLocationID(populationData.destination);
        movementLabel.Text = $"{currentLocationCountyData.countyName} -> {destinationLocationCountyData.countyName}";
        movementActivityHBoxContainer.Show();
    }
    private void GetSecondaryCheckboxes()
    {
        foreach (CheckBox checkBox in secondaryActivitiesHBoxContainer.GetChildren().Cast<CheckBox>())
        {
            secondaryCheckBoxesList.Add(checkBox);
        }
    }

    private void HeroButtonOnPressed()
    {
        PopulationDescriptionControl.Instance.populationData = populationData;
        CountyInfoControl.Instance.populationDescriptionControl.Show();
        if (CountyInfoControl.Instance.populationDescriptionControl.Visible == true)
        {
            PopulationDescriptionControl.Instance.UpdateDescriptionInfo();
        }

        CountyInfoControl.Instance.populationListMarginContainer.Hide();
        PopulationDescriptionControl.Instance.heroButtonClicked = true;
    }

    private void DeselectAllOtherCheckBoxes(int numberOfCheckBox)
    {
        foreach (CheckBox checkBox in heroCheckBoxesList)
        {
            if (checkBox == heroCheckBoxesList[numberOfCheckBox])
            {
                continue;
            }

            checkBox.ButtonPressed = false;
        }
    }

    // I wonder if the populationData.IsHeroSpawned is needed anymore.
    private void SpawnHeroCheckButton()
    {
        GD.Print("Spawned Hero Check Button Global Token: " + Globals.Instance.heroToken);

        if (spawnHeroButton.ButtonPressed == true && populationData.IsHeroSpawned() == false)
        {
            // Assign to Currently Selected Hero so it is ready to be moved.
            Globals.Instance.SelectedCountyPopulation
                = TokenSpawner.Spawn(Globals.Instance.SelectedLeftClickCounty, populationData);
            GD.Print("Spawn Hero Check Box " + Globals.Instance.SelectedCountyPopulation.firstName);
            GD.Print($"{populationData.firstName} token is: {populationData.heroToken}");
            CountyInfoControl.Instance.UpdateEverything();

            GD.Print("Toggled: " + spawnHeroButton.ButtonPressed);
        }
        else
        {
            TokenSpawner.Unspawn(Globals.Instance.SelectedLeftClickCounty, populationData);
            GD.Print("Else Toggled: " + spawnHeroButton.ButtonPressed);
        }
    }

    /// <summary>
    /// The int bound in the signal is the equivalent to the enum in the case.
    /// </summary>
    /// <param name="numberOfCheckBox"></param>
    private void HeroActivitiesCheckBoxPressed(int numberOfCheckBox)
    {
        if (heroCheckBoxesList[numberOfCheckBox].ButtonPressed == false)
        {
            populationData.UpdateActivity(AllEnums.Activities.Idle);
            populationData.currentCountyImprovement?.RemovePopulationFromPopulationAtImprovementList(populationData);
            GD.Print("Scavenge has been unpressed.");
            return;
        }

        DeselectAllOtherCheckBoxes(numberOfCheckBox);

        switch (numberOfCheckBox)
        {
            // Scavenge
            case 0:
                populationData.UpdateActivity(AllEnums.Activities.Scavenge);
                GD.Print("Scavenge has been pressed.");
                return;
            // Build
            case 1:
                populationData.UpdateActivity(AllEnums.Activities.Build);
                return;
            // Work
            case 2:
                populationData.UpdateActivity(AllEnums.Activities.Work);
                return;
            // Research
            case 3:
                populationData.UpdateActivity(AllEnums.Activities.Research);
                return;
            // Explore
            case 4:
                populationData.UpdateActivity(AllEnums.Activities.Explore);
                return;
            default:
                GD.Print("Some hero activity doesn't exist.");
                return;
        }
    }

    private void AssignBuildingCountyImprovement()
    {
        County county = (County)Globals.Instance.countiesParent.GetChild(populationData.location);
        CountyData countyData = county.countyData;
        // Add hero to possible workers list.
        countyData.AddPopulationDataToPossibleWorkersList(populationData);
    }

    // This needs to be moved into the switch above.
    private void ResearchCheckBoxToggled(bool toggled)
    {
        //GD.Print("Research Checkbox Toggled: " + toggled);
        if (toggled == false)
        {
            Research research = new();
            research.RemoveResearcher(populationData);
        }
    }

    // This needs to be on this script because this is also instantiated.
    private static void OnMouseEnteredUI()
    {
        PlayerControls.Instance.stopClickThrough = true;
        //GD.Print("Mouse Over UI: " + PlayerControls.Instance.stopClickThrough);
    }

    private static void OnMouseExitedUI()
    {
        PlayerControls.Instance.stopClickThrough = false;
        //GD.Print("Mouse Over UI: " + PlayerControls.Instance.stopClickThrough);
    }
}
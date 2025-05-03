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
    private readonly List<CheckBox> primaryCheckBoxesList = [];
    private readonly List<CheckBox> secondaryCheckBoxesList = [];

    public override void _Ready()
    {
        // Skips all this stuff if it is the Selected Hero Panel.
        if (primaryActivitiesHBoxContainer == null)
        {
            return;
        }

        GetPrimaryActivityCheckboxes();
        GetSecondaryActivityCheckboxes();
        ConnectButtonSignals();
        PopulateHeroPanel();
    }


    private void PopulateHeroPanel()
    {
        CountyData locationCountyData = Globals.Instance.GetCountyDataFromLocationId(populationData.location);

        // This needs to be up here because we are fucking with the spawnHeroButton below.
        SetDefaultUi();

        UpdateHeroNameAndIcons();

        // Change color of the panel to the faction color.
        SelfModulate = populationData.factionData.factionColor;

        CheckForAvailableActivities(locationCountyData);

        PopulateActivityHBoxes();

        // Check to see if the hero is part of the player's faction to determine what to show.
        // Once we add the ability for heroes to do things in enemy faction counties, we will change this.
        // Currently, we are just making it so that the heroes Activities boxes are hidden.
        // Check if the hero is not player owned.
        if (Globals.Instance.CheckIfPlayerFaction(populationData.factionData) == false)
        {
            heroDescriptionButton.Disabled = true;
            spawnHeroButton.Hide();
            return; // TODO This used to be inside a loop.
        }

        // This checks if the location of the hero is in a non-player owned county.
        // If this is a player hero, but in an enemy's county they can't currently do anything.
        if (Globals.Instance.CheckIfPlayerFaction(populationData.factionData) &&
            Globals.Instance.CheckIfPlayerFaction(locationCountyData.factionData) == false)
        {
            spawnHeroButton.Disabled = true;
            heroDescriptionButton.Disabled = false;
            return; // TODO Test this return.
        }

        if (populationData.IsThisAnArmy() || populationData.activity == AllEnums.Activities.Recruit)
        {
            secondaryActivitiesHBoxContainer.Show();
        }
        else if (populationData.activity != AllEnums.Activities.Move)
        {
            primaryActivitiesHBoxContainer.Show();
        }

        heroDescriptionButton.Disabled = false;
        spawnHeroButton.Show();

        //GD.Print("County Info Control Hero Token: " + populationData.heroToken);
        // This is only for the player's tokens.
        spawnHeroButton.ButtonPressed = populationData.heroToken != null;
    }

    public void UpdateHeroNameAndIcons()
    {
        heroNameLabel.Text =
            $"{populationData.firstName} {populationData.lastName}";

        // Update the icons for each hero.
        switch (populationData)
        {
            case { HeroType: AllEnums.HeroType.FactionLeader }: // FactionLeader
                factionLeaderTextureRect.Show();
                aideTextureRect.Hide();
                armyLeaderTextureRect.Hide();
                break;

            case { HeroType: AllEnums.HeroType.FactionLeaderArmyLeader }: // FactionArmyLeader
                factionLeaderTextureRect.Show();
                aideTextureRect.Hide();
                armyLeaderTextureRect.Show();
                break;

            case { HeroType: AllEnums.HeroType.Aide }: // Aide
                factionLeaderTextureRect.Hide();
                aideTextureRect.Show();
                armyLeaderTextureRect.Hide();
                break;

            case { HeroType: AllEnums.HeroType.ArmyLeader }: // ArmyLeader
                factionLeaderTextureRect.Hide();
                aideTextureRect.Hide();
                armyLeaderTextureRect.Show();
                break;
        }
    }

    private void CheckForAvailableActivities(CountyData countyData)
    {
        DisableMostActivityCheckboxes();
        foreach (CountyImprovementData countyImprovementData in countyData.completedCountyImprovementList)
        {
            // Work
            if (countyImprovementData.maxWorkers > 0
                && countyImprovementData.factionResourceType != AllEnums.FactionGoodType.Research)
            {
                primaryCheckBoxesList[2].Disabled = false;
            }

            primaryCheckBoxesList[2].Disabled = countyImprovementData.CheckIfStatusLowStockpiledGoods();
        }

        // Build
        if (countyData.underConstructionCountyImprovementList.Count > 0)
        {
            primaryCheckBoxesList[1].Disabled = false;
        }
    }

    private void PopulateActivityHBoxes()
    {
        switch (populationData.activity)
        {
            case AllEnums.Activities.Scavenge:
                primaryCheckBoxesList[0].ButtonPressed = true;
                break;
            case AllEnums.Activities.Build:
                primaryCheckBoxesList[1].ButtonPressed = true;
                break;
            case AllEnums.Activities.Work:
                primaryCheckBoxesList[2].ButtonPressed = true;
                break;
            case AllEnums.Activities.Research:
                primaryCheckBoxesList[3].ButtonPressed = true;
                break;
            case AllEnums.Activities.Explore:
                primaryCheckBoxesList[4].ButtonPressed = true;
                break;
            case AllEnums.Activities.Recruit:
                secondaryCheckBoxesList[0].ButtonPressed = true;
                secondaryCheckBoxesList[0].Disabled = false;
                break;
            case AllEnums.Activities.Move:
                ShowMovementActivityHBoxContainer();
                break;
            case AllEnums.Activities.Combat:
            case AllEnums.Activities.Idle:
            case AllEnums.Activities.Service:
            default:
                //GD.Print("PopulateActivitiesHBox has unwritten activities.");
                break;
        }
    }

    /// <summary>
    /// Disables all checkboxes except scavenge.
    /// This is dumb.
    /// </summary>
    private void DisableMostActivityCheckboxes()
    {
        foreach (CheckBox checkBox in primaryCheckBoxesList.Where(checkBox => checkBox != primaryCheckBoxesList[0]
                                                                              && checkBox != primaryCheckBoxesList[3]
                                                                              && checkBox != primaryCheckBoxesList[4]))
        {
            checkBox.Disabled = true;
        }
    }

    private void SetDefaultUi()
    {
        spawnHeroButton.Disabled = false;
        primaryActivitiesHBoxContainer.Hide();
        secondaryActivitiesHBoxContainer.Hide();
        movementActivityHBoxContainer.Hide();
    }

    private void ConnectButtonSignals()
    {
        primaryCheckBoxesList[0].Pressed += () => PrimaryActivitiesCheckBoxPressed(0);
        primaryCheckBoxesList[1].Pressed += () => PrimaryActivitiesCheckBoxPressed(1);
        primaryCheckBoxesList[2].Pressed += () => PrimaryActivitiesCheckBoxPressed(2);
        secondaryCheckBoxesList[0].Pressed += OnRecruitingCheckBoxPressed;
    }

    private void OnRecruitingCheckBoxPressed()
    {
        if (secondaryCheckBoxesList[0].ButtonPressed) return;
        populationData.numberOfSubordinatesWanted = populationData.heroSubordinates.Count;
        secondaryCheckBoxesList[0].Disabled = true;
    }

    private void HidePrimaryActivitiesHBoxContainer()
    {
        primaryActivitiesHBoxContainer.Hide();
    }

    private void HideSecondaryActivitiesHBoxContainer()
    {
        secondaryActivitiesHBoxContainer.Hide();
    }

    private void HideMovementActivityHBoxContainer()
    {
        movementActivityHBoxContainer.Hide();
    }

    private void GetPrimaryActivityCheckboxes()
    {
        foreach (CheckBox checkBox in primaryActivitiesHBoxContainer.GetChildren().Cast<CheckBox>())
        {
            primaryCheckBoxesList.Add(checkBox);
        }
    }

    private void GetSecondaryActivityCheckboxes()
    {
        foreach (CheckBox checkBox in secondaryActivitiesHBoxContainer.GetChildren().Cast<CheckBox>())
        {
            secondaryCheckBoxesList.Add(checkBox);
        }
    }

    public void ShowMovementActivityHBoxContainer()
    {
        Label movementLabel = (Label)movementActivityHBoxContainer.GetChild(0);
        CountyData currentLocationCountyData = Globals.Instance.GetCountyDataFromLocationId(populationData.location);
        CountyData destinationLocationCountyData =
            Globals.Instance.GetCountyDataFromLocationId(populationData.destination);
        movementLabel.Text = currentLocationCountyData != destinationLocationCountyData
            ? $"{currentLocationCountyData.countyName} -> {destinationLocationCountyData.countyName}"
            : $"{Tr("PHRASE_RETURNING_TO")} -> {destinationLocationCountyData.countyName}";

        movementActivityHBoxContainer.Show();
    }

    private void HeroButtonOnPressed()
    {
        PopulationDescriptionControl.Instance.populationData = populationData;
        CountyInfoControl.Instance.populationDescriptionControl.Show();
        if (CountyInfoControl.Instance.populationDescriptionControl.Visible)
        {
            PopulationDescriptionControl.Instance.UpdateDescriptionInfo();
        }

        CountyInfoControl.Instance.populationListMarginContainer.Hide();
        PopulationDescriptionControl.Instance.heroButtonClicked = true;
    }

    private void DeselectAllOtherCheckBoxes(int numberOfCheckBox)
    {
        foreach (CheckBox checkBox in primaryCheckBoxesList)
        {
            if (checkBox == primaryCheckBoxesList[numberOfCheckBox])
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

        if (spawnHeroButton.ButtonPressed && populationData.IsHeroSpawned() == false)
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
    private void PrimaryActivitiesCheckBoxPressed(int numberOfCheckBox)
    {
        if (primaryCheckBoxesList[numberOfCheckBox].ButtonPressed == false)
        {
            populationData.UpdateActivity(AllEnums.Activities.Idle);
            populationData.currentCountyImprovement?.RemovePopulationFromPopulationAtImprovementList(populationData);
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
        // Add the hero to the possible workers list.
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
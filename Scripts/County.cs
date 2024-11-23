using Godot;
using System.Collections.Generic;

namespace PlayerSpace;

public partial class County : Node2D
{
    [Export] public CountyData countyData;

    [ExportGroup("Attached Nodes")]
    [Export] public Sprite2D maskSprite;
    [Export] public Sprite2D countySprite;
    [Export] public Sprite2D capitalSprite;
    [Export] public Node2D countyOverlayNode2D;
    [Export] public Node2D heroSpawn;
    [Export] public BattleControl battleControl;

    [Export] public HBoxContainer armiesHBox;
    [Export] public HBoxContainer heroesHBox;

    private SelectToken selectToken;

    public List<County> neighborCounties = [];

    public override void _Ready()
    {
        countyData.countyNode = this;

        // This is here so that it doesn't subscribe to the clock when the Map Editor is running.
        if (GetTree().CurrentScene.SceneFilePath == "res://Scenes/Main.tscn")
        {
            Clock.Instance.SetDay += EndOfDay;
            Clock.Instance.SetDay += DayStart;
        }
    }

    private void EndOfDay()
    {
        CountyAI countyAI = new();

        // GD.Print("County Hour One.");
        // Subtract county resources yesterday from today.
        countyData.SubtractCountyResources();

        // Copy the county resources to yesterday.
        countyData.CopyCountyResourcesToYesterday(); // We will use this data to update the numbers on the top bar all day.

        // Check to see if any population needs healing from starvation or whatever.
        CountyData.CheckForHealing(countyData.countyPopulationList);
        CountyData.CheckForHealing(countyData.herosInCountyList);
        CountyData.CheckForHealing(countyData.armiesInCountyList);

        // Shouldn't this be at the beginning of the day??
        countyAI.DecideBuildingCountyImprovements(this);

        // Generates resources through work, scavenging, building, and research.
        // Generates the daily amount of work/building for each county improvement per person.
        PopulationWork.WorkDayOverForPopulation(countyData);
        // Converts the totaly daily amount of work into goods and construction.  Possibly scavenging and research eventually.
        PopulationWork.CalculateWorkToGoodsProduction(countyData);

        PopulationAI.IsThereEnoughFood(countyData); // This is a terrible name for this method.

        // This is a check for Occational needs.
        // Population uses other resources besides food.
        countyData.OccationalNeeds();

        // See if we can combine this into something else.
        countyData.CheckIfCountyImprovementsAreDone();

        // Passive research for each county population, not including heroes.
        Research.AssignPassiveResearch(countyData.factionData, countyData.countyPopulationList);

        // Update all the top bar resources
        TopBarControl.Instance.UpdateResourceLabels();

        // Update the county info control with the counties available resources.
        if (Globals.Instance.SelectedLeftClickCounty != null)
        {
            CountyInfoControl.Instance.UpdateCountyAvailableResources();
        }
    }

    private void DayStart()
    {
        // Assign people to the prioritized county improvements.
        countyData.AssignPeopleToPrioritizedImprovements();
        // Gets all the idle people and puts them in a list for the next methods.
        countyData.FindIdlePopulation();
        // We may want construction to come before work, so that people will build stuff vs always be working
        // and never build anything.
        countyData.CheckForConstruction();
        countyData.CheckForPreferredWork();
        countyData.CheckForAnyWork();

        // Sets people to scavenge.
        countyData.CheckForScavengingFood();
        countyData.CheckForScavengingRemnants();

        // Counts the idle works and sets the idleWorkers variable in the County Data.
        countyData.CountIdleWorkers();
    }

    private void OnTreeExit()
    {
        Clock.Instance.SetDay -= EndOfDay;
        Clock.Instance.SetDay -= DayStart;
    }
}


using Godot;
using System.Collections.Generic;
using System.Linq;

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

    private HeroToken selectToken;

    public List<County> neighborCounties = [];

    public override void _Ready()
    {
        countyData.countyNode = this;

        // This is here so that it doesn't subscribe to the clock when the Map Editor is running.
        if (GetTree().CurrentScene.SceneFilePath == "res://Scenes/Main.tscn")
        {
            Clock.Instance.DailyHourOne += EndOfDay;
            Clock.Instance.Weekly += Weekly;
            Clock.Instance.DailyHourThree += StartDay;
        }
    }

    private void Weekly()
    {
        GD.PrintRich($"[rainbow]County : Weekly!!!!!");
        // Check loyalty and update work status if necessary.
        PopulationWork.WorkWeekOverForPopulation(countyData.populationDataList);
    }

    private void EndOfDay()
    {
        GD.PrintRich($"[rainbow]County : EndOfDay!!!!!");

        CountyAI countyAI = new();

        // GD.Print("County Hour One.");
        // Subtract county resources yesterday from today.
        countyData.SubtractCountyResources();

        // Copy the county resources to yesterday.
        countyData.CopyCountyResourcesToYesterday(); // We will use this data to update the numbers on the top bar all day.

        // Check to see if any population needs healing from starvation or whatever.
        CountyData.CheckForHealing(countyData.populationDataList);
        CountyData.CheckForHealing(countyData.heroesInCountyList);
        CountyData.CheckForHealing(countyData.armiesInCountyList);

        // It checks this at end of day, so that at day start all of the employment etc hits.
        countyAI.DecideBuildingCountyImprovements(this);

        // Goes through each hero just like if they were a normal county population and
        // generates their work amount/research.
        PopulationWork.WorkDayOverForPopulation(countyData, countyData.heroesInCountyList);

        // Generates resources through work, scavenging, building, and research.
        // Generates the daily amount of work amount/building for each county improvement per person.
        PopulationWork.WorkDayOverForPopulation(countyData, countyData.populationDataList);

        // Converts the totaly daily amount of work into goods and construction.
        // Possibly scavenging and research eventually.
        PopulationWork.CalculateWorkToGoodsProduction(countyData);

        PopulationAI.IsThereEnoughFood(countyData); // This is a terrible name for this method.

        // This is a check for Occational needs.
        // Population uses other resources besides food.
        countyData.OccationalNeeds();

        // See if we can combine this into something else.
        GD.Print("County: " + countyData.countyName);
        countyData.CheckIfCountyImprovementsAreDone();

        // Update all the top bar resources
        TopBarControl.Instance.UpdateTopBarGoodLabels();

        // Update the county info control with the counties available resources.
        if (Globals.Instance.SelectedLeftClickCounty != null)
        {
            CountyInfoControl.Instance.UpdateCountyAvailableResources();
        }
    }

    private void StartDay()
    {
        GD.PrintRich($"[rainbow]County : StartOfDay!!!!!");

        // Prioritized County Improvements needs to go first.
        // County Improvements gather goods for their stockpile.
        // Sorts the list first by prioritized, then gathers the stockpiled goods.  Written by ChatGPT.
        foreach (CountyImprovementData countyImprovementData in countyData.completedCountyImprovements
            .OrderByDescending(c => c.prioritize))
        {
            Haulmaster.GatherStockpileGoods(countyData, countyImprovementData);
        }

        // Assign people to the prioritized county improvements.
        countyData.AssignPeopleToPrioritizedImprovements();

        // Gets all the idle people and puts them in a list for the next methods.
        countyData.FindIdlePopulation();

        countyData.CheckForPreferredWork();

        // We may want construction to come before work, so that people will build stuff vs always be working
        // and never build anything.
        countyData.CheckForConstruction();
        countyData.CheckForAnyWork();

        // Sets people to scavenge.
        countyData.CheckForScavengingFood();
        countyData.CheckForScavengingRemnants();

        // Counts the idle works and sets the idleWorkers variable in the County Data.
        // This is for the player UI mostly.
        countyData.CountIdleWorkers();
    }

    private void OnTreeExit()
    {
        Clock.Instance.DailyHourOne -= EndOfDay;
        Clock.Instance.DailyHourOne -= StartDay;
    }
}


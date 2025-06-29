using Godot;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace;

public partial class County : Node2D
{
    [Export] public CountyData countyData;

    [ExportGroup("Attached Nodes")] [Export]
    public Sprite2D maskSprite;

    [Export] public Sprite2D countySprite;
    [Export] public Sprite2D capitalSprite;
    [Export] public Node2D countyOverlayNode2D;
    [Export] public Node2D heroSpawn;
    [Export] public BattleControl battleControl;

    [Export] public HBoxContainer armiesHBox;
    [Export] public HBoxContainer heroesHBox;

    private HeroToken selectToken;

    public readonly List<County> neighborCounties = [];

    public override void _Ready()
    {
        countyData.countyNode = this;

        // This is here so that it doesn't subscribe to the clock when the Map Editor is running.
        if (GetTree().CurrentScene.SceneFilePath == "res://Scenes/Main.tscn")
        {
            Clock.Instance.DailyHourZeroFirstQuarter += EndOfDay;
            Clock.Instance.DailyHourZeroSecondQuarter += StartDay;
            Clock.Instance.DailyHourZeroFourthQuarter += AfterStartDay;
            Clock.Instance.Weekly += Weekly;
        }
    }

    /// <summary>
    /// Fires once a week and on the first day of the game.
    /// </summary>
    private void Weekly()
    {
        //GD.PrintRich($"[rainbow]County : Weekly!!!!!");
        // Check loyalty and update work status if necessary.
        PopulationWorkEnd.WorkWeekOverForPopulation(countyData.populationDataList);
    }

    private void EndOfDay()
    {
        //GD.PrintRich($"[rainbow]County : EndOfDay!!!!!");

        CountyAI countyAi = new();

        // Check each hero to see if they need recruit subordinates.  I moved this from StartOfDay on 5/18/25
        Recruiter.CheckForRecruitment(countyData);

        // GD.Print("County Hour One.");
        // Subtracted county resources yesterday from today.
        countyData.SubtractCountyResources();

        // Copy the county resources to yesterday.
        countyData.CopyCountyGoodsToYesterday(); // We will use this data to update the numbers on the top bar all day.

        // Check to see if any population needs healing from starvation or whatever.
        CountyData.CheckForHealing(countyData.populationDataList);
        CountyData.CheckForHealing(countyData.heroesInCountyList);
        CountyData.CheckForHealing(countyData.armiesInCountyList);

        // It checks this at end of day, so that at day start all the employment etc. hits.
        countyAi.DecideBuildingCountyImprovements(this);

        // Hero
        // Goes through each hero just like if they were a normal county population and
        // generates their work amount/research.
        PopulationWorkEnd.WorkDayOverForPopulation(countyData, countyData.heroesInCountyList);

        // Population
        // Generates resources through work, scavenging, building, and research.
        // Generates the daily amount of work amount/building for each county improvement per person.
        PopulationWorkEnd.WorkDayOverForPopulation(countyData, countyData.populationDataList);

        // Converts the totally daily amount of work into goods and construction.
        // Possibly scavenging and research eventually.
        PopulationWorkEnd.CalculateWorkToGoodsProduction(countyData);

        PopulationAI.IsThereEnoughFood(countyData); // This is a terrible name for this method.

        // This is a check for Occasional needs.
        // The Population uses other goods besides food.
        countyData.OccasionalNeeds();

        // Check to see how long each recruit has been recruited before they enter service for real.
        Recruiter.CheckForDaysRecruited(countyData);

        // See if we can combine this into something else.
        GD.Print("County: " + countyData.countyName);
        countyData.CheckIfCountyImprovementsAreDone();

        // Update all the top bar resources
        TopBarControl.Instance.UpdateTopBarGoodLabels();

        // Update the county info control with the counties available resources.
        if (Globals.Instance.SelectedLeftClickCounty != null)
        {
            CountyInfoControl.Instance.UpdateCountyAvailableScavengeableGoods();
        }

        countyData.ClearIdlePopulationList();
    }

    private void StartDay()
    {
        //GD.PrintRich($"[rainbow]{countyData.countyName} : StartOfDay!!!!!");

        // Prioritized County Improvements needs to go first.
        // County Improvements gather goods for their stockpile.
        // Sorts the list first by prioritized, then gathers the stockpiled goods.  Written by ChatGPT.
        foreach (CountyImprovementData countyImprovementData in countyData.completedCountyImprovementList
                     .OrderByDescending(c => c.prioritize))
        {
            Haulmaster.GatherStockpileGoods(countyData, countyImprovementData);
        }

        // Add heroes to both prioritized hero builders and prioritized hero worker lists.
        // We are doing this first because there aren't that many heroes that should be working or building.
        HeroWorkStart.AssignWorkingHeroesToPrioritizedLists(countyData);

        // If there is no prioritized construction or building, skip making the list.
        // Check for prioritized under construction improvements
        PopulationWorkStart.GeneratePrioritizedConstructionImprovementList(countyData);
        GD.Print($"{countyData.countyName} : Prioritized Construction Improvement List Count: " +
                 countyData.prioritizedConstructionImprovementList.Count);
        if (countyData.prioritizedConstructionImprovementList.Count > 0)
        {
            PopulationWorkStart.GeneratePrioritizedBuildersList(countyData);

            // Adds builders and heroes to improvement
            PopulationWorkStart.AssignBuildersToImprovement(countyData);

            countyData.prioritizedConstructionImprovementList.Clear();
        }

        // We don't need the prioritized builders list anymore, because we generate the idlePopulationList after this.
        PopulationWorkStart.ClearPrioritizedBuildersList(countyData);

        // Create a list of prioritized working improvement.
        PopulationWorkStart.GeneratePrioritizedWorkImprovementList(countyData);

        // Assign people to the prioritized work improvements.
        GD.Print($"{countyData.countyName} : Prioritized Work Improvement List Count: " +
                 countyData.prioritizedWorkImprovementList.Count);
        if (countyData.prioritizedWorkImprovementList.Count > 0)
        {
            PopulationWorkStart.GeneratePrioritizedWorkersList(countyData);

            // Adds workers and heroes to improvement
            PopulationWorkStart.AssignWorkersToImprovement(countyData);

            countyData.prioritizedWorkImprovementList.Clear();
        }

        // Clear the prioritized workers lists.
        // We don't need the prioritized workers list anymore, because we generate the idlePopulationList after this.
        PopulationWorkStart.ClearPrioritizedWorkersList(countyData);

        // Gets all the idle people and puts them in a list for the next methods.
        countyData.FindIdlePopulation();

        // Currently, construction is first to everything gets built first.
        // Heroes work, or building depends on the player.
        countyData.AssignEveryoneToConstruction();

        countyData.CheckForPreferredWork();

        countyData.CheckForAnyWork();

        // Sets people to scavenge.
        countyData.CheckForScavengingFood();
        countyData.CheckForScavengingRemnants();

        // Counts the idle works and sets the idleWorkers variable in the County Data.
        // This is for the player UI mostly.
        countyData.CountIdleWorkers();
    }

    /// <summary>
    /// This equips all the heroes in 
    /// </summary>
    private void AfterStartDay()
    {
        EquipEachHero();
        CountyInfoControl.Instance.UpdateEverything();
    }

    /// <summary>
    /// Equips the heroes that are in friendly counties and that aren't moving.
    /// This needs to be MoveToken != true because of the null check on heroToken.
    /// </summary>
    private void EquipEachHero()
    {
        if (countyData.heroesInCountyList == null)
        {
            return;
        }

        foreach (PopulationData populationData in countyData.heroesInCountyList)
        {
            if (countyData.factionData == populationData.factionData
                && populationData.heroToken?.tokenMovement.MoveToken != true)
            {
                Quartermaster.EquipHeroesAndSubordinates(populationData);
            }
            else
            {
                GD.Print("Hero is in an unfriendly county so it needs to equip from supply, which isn't implemented yet.");
            }
        }
    }

    private void OnTreeExit()
    {
        Clock.Instance.DailyHourZeroFirstQuarter -= EndOfDay;
        Clock.Instance.DailyHourZeroFirstQuarter -= StartDay;
    }
}
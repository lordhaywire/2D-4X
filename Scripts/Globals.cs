using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoloadSpace;

namespace PlayerSpace;

public partial class Globals : Node
{
    public static Globals Instance { get; private set; }
    public readonly Random random = new();

    [Export] public bool loadGameAtStart;
    
    [ExportGroup("Player Faction BS")]
    [Export] public FactionData playerFactionData;

    //[Export] public Godot.Collections.Array<FactionData> allFactionData;// = [];

    [ExportGroup("Faction Variables")]
    [Export] public int dailyInfluenceGain;
    [Export] public int leaderOfPeopleInfluenceBonus;

    [ExportGroup("Game Settings")]
    [Export] public bool startPaused;
    [Export] public bool turnOffStoryEvents;
    [Export] public bool winAllBattles;

    [ExportGroup("Selected Items")]
    [Export] public int selectedCountyId = -1;
    [Export] private County selectedLeftClickCounty;

    public County SelectedLeftClickCounty
    {
        get => selectedLeftClickCounty;
        set
        {
            if (selectedLeftClickCounty != null)
            {
                selectedLeftClickCounty.maskSprite.SelfModulate = new Color(1, 1, 1);
            }
            selectedLeftClickCounty = value;
            if (selectedLeftClickCounty != null)
            {
                selectedLeftClickCounty.maskSprite.SelfModulate = new Color(0, 0, 0);
            }
        }
    }
    [Export] public County selectedRightClickCounty;
    private PopulationData selectedCountyPopulation;
    public PopulationData SelectedCountyPopulation
    {
        get => selectedCountyPopulation;
        set
        {
            selectedCountyPopulation = value;
            if (selectedCountyPopulation == null)
            {
                PlayerUICanvas.Instance.selectedHeroPanelContainer.Hide();
            }
            else
            {
                CountyInfoControl.UpdateSelectedHero();
                PlayerUICanvas.Instance.selectedHeroPanelContainer.Show();
            }
        }
    }

    public CountryImprovementPanelContainer selectedPossibleBuildingControl;
    public bool isVisitorList;

    [ExportGroup("Map")]
    [Export] public string pathToCounties = "res://Counties/";
    [Export] public Texture2D mapColorCoded;

    [ExportGroup("Population Generation")]
    [Export] public Node2D countiesParent; // Used for Population generation and random color.  I think we are going to change how the colors are distributed.
    //[Export] public int heroPopulation = 1;
    [Export] public int totalCapitolPop = 20;
    [Export] public int minimumCountyPop = 1;
    [Export] public int maximumCountyPop = 4;

    [ExportGroup("Faction Shit")]
    [Export] public Node factionsParent;
    [Export] public int minimumFood;

    [ExportGroup("County Stuff")]
    // These two are populated from AllResources at Ready.
    [Export] public int numberOfPerishableGoods; // Total perishable goods
    [Export] public int numberOfNonperishableGoods; // Total nonperishable goods
    [Export] public int maxScavengeableScrap = 10000;
    [Export] public int maxScavengeableFood = 10000;
    [Export] public int startingPerishableStorage = 2000;
    [Export] public int startingNonperishableStorage = 2000;
    [Export] public int startingAmountOfEachGood = 100;
    
    [ExportGroup("Exploration Variables")]
    [Export] public int numberOfPrimaryTerrainEvents = 10;
    [Export] public int numberOfSecondaryTerrainEvents = 6;
    [Export] public int numberOfTertiaryTerrainEvents = 3;
    [Export] public int explorationCost = 10;
    [Export] public int subordinateExplorationBonus = 5;
    
    [ExportGroup("County Improvement Stuff")]
    [Export] public int minDaysStockpile = 2; // The amount of input goods (days) an improvement tries to hold.
    [Export] public int maxDaysStockpile = 7; // The max amount (in days) of goods a county improvement will try to stockpile
    
    [ExportGroup("Population Work")]
    [Export] public int dailyScavengedAmount = 2;
    [Export] public int dailyScavengedAmountBonus = 1;
    [Export] public int dailyWorkAmount = 10; // This is for work and construction.
    [Export] public int dailyWorkAmountBonus = 5; // This is for work and construction.
    [Export] public int foodToGainHappiness = 3;
    [Export] public int foodToGainNothing = 2;
    [Export] public int foodToLoseHappiness = 1;
    [Export] public int occasionalResourceUsageAmount = 1;
    [Export] public int occasionalNeedIncreaseAmount = 5;
    [Export] public int daysEmployedBeforeLoyaltyCheck = 7;
    [Export] public int daysEmployedIdleBeforeLookingForNewWork = 7;

    [ExportGroup("Population Shit")]
    [Export] public PackedScene heroToken;
    [Export] public PackedScene spawnedTokenButton;
    [Export] public int movementSpeed = 10;
    [Export] public float heroStackingOffset = 3;
    [Export] public Vector2 heroMoveTarget;
    [Export] public int costOfHero;
    [Export] public int loyaltyCheckNumber = 50; // Battle loyalty
    [Export] public int willWorkLoyalty = 20; // The loyalty a population needs to be willing to work.
                                              // 50 is too high for testing, but might work well for the real game.
    [Export] public int willFightLoyalty = 30; // Loyalty required for a population to be able to be hired as a subordinate.
    [Export] public int startingHitPoints = 10;
    [Export] public int fastLearningNeeded = 10;
    [Export] public int mediumLearningNeeded = 50;
    [Export] public int slowLearningNeeded = 100;
    [Export] public int maxXpRoll = 5; // One above max.
    [Export] public int moraleDamageMin = 1;
    [Export] public int moraleDamageMax = 21; // One above max.
    [Export] public int moraleRecoveryMin = 1;
    [Export] public int moraleRecoveryMax = 11; // One above max.
    [Export] public int researcherResearchIncrease = 10;
    [Export] public int researchIncreaseBonus = 5; // One above max.
    [Export] public int passiveResearchIncrease = 2;
    [Export] public int passiveResearchBonus = 1;
    [Export] public int daysUntilDamageFromStarvation = 15;
    [Export] public int minDaysUntilServiceStarts = 1;
    [Export] public int maxDaysUntilServiceStarts = 8; // One above max.
    [Export] public int foodBeforeScavenge = 500; // Less than this amount will make people scavenge.
    [Export] public int remnantsBeforeScavenge = 500; // Less than this amount will make people scavenge.

    private int researchClicked; // This is so the Research description panel knows which research was clicked.

    private string listsPath = "Lists/";
    private string maleNamesPath = "MaleNames.txt";
    private string femaleNamesPath = "FemaleNames.txt";
    private string lastNamesPath = "LastNames.txt";

    public readonly List<string> maleNames = [];
    public readonly List<string> femaleNames = [];
    public readonly List<string> lastNames = [];

    [ExportGroup("This is some bullshit.")]
    [Export] public bool isInsideToken;

    // ReSharper disable once CollectionNeverQueried.Global
    public readonly List<FactionData> deadFactions = [];

    public override void _Ready()
    {
        Instance = this;
        //allFactionData = [];
        LoadNames();
        CountGoods();
    }

    public async Task WaitFrames(int frameCount)
    {
        for (int i = 0; i < frameCount; i++)
        {
            await ToSignal(GetTree(), "process_frame");
        }
    }

    public CountyData GetCountyDataFromLocationId(int location)
    {
        County county = (County)countiesParent.GetChild(location);
        return county.countyData;
    }

    public bool CheckIfPlayerFaction(FactionData factionData)
    {
        if(factionData == playerFactionData)
        {
            return true;
        }
        return false;
    }
    private void LoadNames()
    {
        // Load all the names from disk.

        // I think the variable can be used if we open up the root directory first.
        // Right now this code is doing nothing except the GD.Print stuff.
        //listDirectory = ProjectSettings.LocalizePath(listsPath); 
        GD.Print(OS.HasFeature("editor") ? "Is in the editor!!!" : "Is not in the editor!");

        //listDirectory = ProjectSettings.LocalizePath(listsPath);
        DirAccess directory = DirAccess.Open("res://");
        if (directory.DirExists("res://Lists/"))
        {
            using var maleFile = FileAccess.Open("res://Lists/MaleNames.txt", FileAccess.ModeFlags.Read);//(listsPath + maleNamesPath, FileAccess.ModeFlags.Read);
            while (maleFile.GetPosition() < maleFile.GetLength())
            {
                maleNames.Add(maleFile.GetLine());
            }
            using var femaleFile = FileAccess.Open("res://Lists/FemaleNames.txt", FileAccess.ModeFlags.Read); //(listsPath + femaleNamesPath, FileAccess.ModeFlags.Read);
            while (femaleFile.GetPosition() < femaleFile.GetLength())
            {
                femaleNames.Add(femaleFile.GetLine());
            }
            using var lastNameFile = FileAccess.Open("res://Lists/LastNames.txt", FileAccess.ModeFlags.Read); //(listsPath + lastNamesPath, FileAccess.ModeFlags.Read);
            while (lastNameFile.GetPosition() < lastNameFile.GetLength())
            {
                lastNames.Add(lastNameFile.GetLine());
            }
            //GD.Print("Names have been loaded.");
        }
        /*
        else
        {
            GD.Print("Directory doesn't exist! " + listDirectory);
        }
        */

    }

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
    
    private void CountGoods()
    {
        int perishable = 0;
        int nonperishable = 0;

        foreach (GoodData resourceData in Autoload.Instance.allGoods)
        {
            switch (resourceData.perishable)
            {
                case AllEnums.Perishable.Perishable:
                    perishable++;
                    break;
                case AllEnums.Perishable.Nonperishable:
                    nonperishable++;
                    break;
            }
        }
        numberOfPerishableGoods = perishable;
        numberOfNonperishableGoods = nonperishable;
    }
}


using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlayerSpace;

public partial class Globals : Node
{
    public static Globals Instance { get; private set; }
    public readonly Random random = new();

    [ExportGroup("Player Faction BS")]
    [Export] public FactionData playerFactionData;

    [Export] public Godot.Collections.Array<FactionData> factionDatas = [];

    [ExportGroup("Faction Variables")]
    [Export] public int dailyInfluenceGain;
    [Export] public int leaderOfPeopleInfluenceBonus;

    [ExportGroup("Game Settings")]
    [Export] public bool startPaused;
    [Export] public bool turnOffStoryEvents = false;

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
                selectedLeftClickCounty.maskSprite.SelfModulate = new Color(1, 1, 1, 1f);
            }
            selectedLeftClickCounty = value;
            if (selectedLeftClickCounty != null)
            {
                selectedLeftClickCounty.maskSprite.SelfModulate = new Color(0, 0, 0, 1f);
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
    [Export] public Node2D countiesParent; // Used for Population generation and random color.  I think we are going to change how the colors are distubuted.
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
    [Export] public int startingPerishableStorage = 500;
    [Export] public int startingNonperishableStorage = 500;
    [Export] public int startingAmountOfEachGood = 100;

    [ExportGroup("County Improvement Stuff")]
    [Export] public int minDaysStockpile = 2; // The amount of input goods (days) a improvement tries to hold.
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

    public List<FactionData> deadFactions = [];

    public override void _Ready()
    {
        Instance = this;
        LoadNames();
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

        //string listDirectory = "";
        // I think the variable can be used, if we open up the root directory first.
        // Right now this code is doing nothing, except the GD.Print stuff.
        if (OS.HasFeature("editor"))
        {
            GD.Print("Is in the editor!!!");
            //listDirectory = ProjectSettings.LocalizePath(listsPath); 
        }
        else
        {
            GD.Print("Is not in the editor!");
            //listDirectory = ProjectSettings.LocalizePath(listsPath);
        }

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

    /*
    public class ListWithNotify<T> : IEnumerable<T> where T : class
    {
        readonly List<T> list = [];

        public delegate void ItemAddedEventHandler(object sender, T item);

        public event ItemAddedEventHandler ItemAdded;
        // This makes this function like a normal list.
        public T this[int i]
        {
            get
            {
                if (list.Count > 0)
                {
                    return list[i];
                }
                else
                {
                    Console.WriteLine("Default: " + default(T));
                    return null;
                }
            }
            set { list[i] = value; }
        }

        // These makes the normal list actions work correctly. If we need to do other things to the list we need to add them here.
        public int Count() // This is not a normal list so when using it, it will need ().
        {
            return list.Count;
        }
        public void Add(T item)
        {
            list.Add(item);
            OnItemAdded(item);
        }

        public void Insert(int i, T item)
        {
            list.Insert(i, item);
            OnItemAdded(item);
        }

        public void Remove(T item)
        {
            list.Remove(item);
        }

        public void RemoveAt(int i)
        {
            list.RemoveAt(i);
        }

        protected virtual void OnItemAdded(T item)
        {
            ItemAdded?.Invoke(this, item);
        }

        // Implementing IEnumerable<T> interface to enable foreach
        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        // Implementing IEnumerable interface
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    */
    public static void OnMouseEnteredUI()
    {
        PlayerControls.Instance.stopClickThrough = true;
        //GD.Print("Mouse Over UI: " + PlayerControls.Instance.stopClickThrough);
    }

    public static void OnMouseExitedUI()
    {
        PlayerControls.Instance.stopClickThrough = false;
        //GD.Print("Mouse Over UI: " + PlayerControls.Instance.stopClickThrough);
    }
}


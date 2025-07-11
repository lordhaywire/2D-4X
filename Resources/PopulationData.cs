using Godot;
using System;

namespace PlayerSpace;

[GlobalClass]
public partial class PopulationData : Resource
{
    [Export] public FactionData factionData;
    [Export] public int location;
    [Export] public int lastLocation;
    [Export] public int destination;

    [ExportGroup("Info")] [Export] public string firstName;
    [Export] public string lastName;
    [Export] public bool isMale;
    [Export] public int age;

    [ExportGroup("Personality")] [Export] public AllEnums.Personality personality;
    public IPersonality iPersonality; // I have fucked my future self.  This will not save with the resource saver.

    [ExportGroup("Hero")]
    // Change this to an enum
    [Export]
    public bool isHero;

    //[Export] public bool isWorker;

    [Export] private AllEnums.HeroType heroType;

    [Export]
    public AllEnums.HeroType HeroType
    {
        get => heroType;
        set
        {
            heroType = value;

            if (heroToken != null)
            {
                AllTokenTextures.Instance.AssignTokenTextures(heroToken);
            }
        }
    }

    [Export] public int numberOfSubordinatesWanted;
    [Export] public Godot.Collections.Array<PopulationData> heroSubordinates; //=[];

    [ExportGroup("Perks")] [Export] public Godot.Collections.Dictionary<AllEnums.Perks, PerkData> perks;

    [ExportGroup("Expendables")] [Export] public int hitPoints;
    [Export] public int maxHitPoints;

    [Export]
    public int
        moraleExpendable; // I think we are going to have this as leader morale or army morale or some shit.

    [Export] public int loyaltyBase;
    [Export] private int loyaltyAdjusted;

    [Export]
    public int LoyaltyAdjusted
    {
        get => loyaltyAdjusted;
        set =>
            // Make it so that loyaltyAdjusted can't go above 100.
            loyaltyAdjusted = Math.Min(value, 100);
    }

    [Export] public int happiness;

    [Export]
    public int Happiness
    {
        get => happiness;
        set
        {
            happiness = Math.Min(value, 100);

            // This is checking happiness as if it was an attribute and adjusting the loyalty by
            // the attribute bonus.  So if the happiness gets really low, the loyal will only ever get a negative 20,
            // or if the happiness is really high, it will only get a +20.
            LoyaltyAdjusted = loyaltyBase + AttributeData.GetAttributeBonus(value, false, false);
            //GD.Print($"{firstName} {lastName} loyalty adjusted: {LoyaltyAdjusted}");
        }
    }

    [Export] public int daysEmployed;
    [Export] public int daysEmployedButIdle;
    [Export] public int daysStarving;
    [Export] public int daysRecruited;
    [Export] public int daysUntilServiceStarts;

    // Resource needs, currently there is just 1 need, Remnants.
    [Export] public Godot.Collections.Dictionary<AllEnums.CountyGoodType, int> needs;

    [ExportGroup("Attributes")] [Export]
    public Godot.Collections.Dictionary<AllEnums.Attributes, AttributeData> attributes; // = [];

    [ExportGroup("Skills")] [Export] public Godot.Collections.Dictionary<AllEnums.Skills, SkillData> skills; // = [];
    [Export] public SkillData preferredSkill;
    [Export] public InterestData interestData;

    [ExportGroup("Work")] [Export] public AllEnums.Activities activity;

    [ExportGroup("Inventory")] [Export] public bool useNewestEquipment;
    [Export] public GoodData[] equipment;

    [Export] public CountyImprovementData currentCountyImprovement; // Used for work and building.

    [Export] public ResearchItemData passiveResearchItemData;
    [Export] public ResearchItemData currentResearchItemData;

    public HeroToken heroToken;

    public bool CheckForPerk(AllEnums.Perks perk)
    {
        if (perks.ContainsKey(perk))
        {
            return true;
        }

        return false;
    }

    // They always need to be a hero first for everything else to work.
    public void ChangeToArmy()
    {
        isHero = true;
        HeroType = HeroType == AllEnums.HeroType.FactionLeader ? AllEnums.HeroType.FactionLeaderArmyLeader : AllEnums.HeroType.ArmyLeader;

        County selectCounty = (County)Globals.Instance.countiesParent.GetChild(location);
        selectCounty.countyData.armiesInCountyList.Add(this);
        selectCounty.countyData.heroesInCountyList.Remove(this);
    }

    public void UpdateActivity(AllEnums.Activities newActivity)
    {
        activity = newActivity;
        
        if (newActivity == AllEnums.Activities.Idle && factionData.isPlayer)
        {
            GD.Print($"{GetFullName()} is set to idle!");
        }

        if (newActivity == AllEnums.Activities.Recruited && factionData.isPlayer)
        {
            GD.Print($"{GetFullName()} is set to recruited!");
        }
    }

    private void UpdateDestination(int newDestination)
    {
        destination = newDestination;
    }

    public void UpdateCurrentCountyImprovement(CountyImprovementData countyImprovementData)
    {
        currentCountyImprovement = countyImprovementData;
    }

    public void AddRandomHappiness(int maxHappiness)
    {
        Random random = new();
        Happiness += random.Next(1, maxHappiness);
        //GD.Print($"Happiness gained: {firstName} {lastName} happiness is now {Happiness}");
    }

    public string GetActivityName()
    {
        string name = TranslationServer.Translate(AllActivities.Instance.allActivityData[(int)activity].name);

        return name;
    }

    public void RemoveRandomHappiness(int maxHappiness)
    {
        Random random = new();
        Happiness -= random.Next(1, maxHappiness);
        //GD.Print($"Happiness lost: {firstName} {lastName} happiness is now {Happiness}");
    }

    public bool CheckWillWorkLoyalty()
    {
        if (LoyaltyAdjusted >= Globals.Instance.willWorkLoyalty)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Checks to see if the token has been instantiated, and if it has, then the hero is considered spawned.
    /// </summary>
    public bool IsHeroSpawned()
    {
        if (heroToken != null)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Makes activity idle and removes the population from the populationAtImprovement list.
    /// </summary>
    public void RemoveFromCountyImprovement()
    {
        if (currentCountyImprovement == null) return;
        GD.Print($"{firstName} was removed from {currentCountyImprovement?.improvementName}");
        UpdateActivity(AllEnums.Activities.Idle);
        currentCountyImprovement?.populationAtImprovement.Remove(this);
        currentCountyImprovement = null;
    }

    public void UpdateCurrentResearch(ResearchItemData researchItemData)
    {
        UpdateActivity(AllEnums.Activities.Research);
        currentResearchItemData = researchItemData;
    }

    public bool IsThisAnArmy()
    {
        if (HeroType == AllEnums.HeroType.FactionLeaderArmyLeader || HeroType == AllEnums.HeroType.ArmyLeader)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Removes the populations research Item Data and sets their activity to Work.
    /// </summary>
    public void RemoveFromResearch()
    {
        currentResearchItemData = null;
        UpdateActivity(AllEnums.Activities.Work);
        // When the AI removes people from research, it is going to try and do this.  I am not sure if we care.
        ResearchControl.Instance.assignedResearchers.Remove(this);
    }

    public string GetFullName()
    {
        string fullName = $"{firstName} {lastName}";
        return fullName;
    }
}
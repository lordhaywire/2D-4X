using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoloadSpace;

namespace PlayerSpace;

[GlobalClass]
public partial class PopulationData : Resource
{
    [Export] public int populationId;
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

    [Export] public int
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

    [Export] private int happiness;

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

    public PopulationDto ToDto()
    {
        return new PopulationDto
        {
            PopulationId = populationId,
            FactionId = factionData?.factionId,
            Location = location,
            LastLocation = lastLocation,
            Destination = destination,

            FirstName = firstName,
            LastName = lastName,
            IsMale = isMale,
            Age = age,

            Personality = personality.ToString(),
            IsHero = isHero,
            HeroType = HeroType.ToString(),
            NumberOfSubordinatesWanted = numberOfSubordinatesWanted,

            HeroSubordinates = heroSubordinates.Select(h => h.populationId).ToList(),
            Perks = perks.ToDictionary(
                kvp => kvp.Key.ToString(),
                kvp => PerkData.ToDto(kvp.Value)),
            HitPoints = hitPoints,
            MaxHitPoints = maxHitPoints,
            MoraleExpendable = moraleExpendable,
            LoyaltyBase = loyaltyBase,
            LoyaltyAdjusted = LoyaltyAdjusted,
            Happiness = Happiness,

            DaysEmployed = daysEmployed,
            DaysEmployedButIdle = daysEmployedButIdle,
            DaysStarving = daysStarving,
            DaysRecruited = daysRecruited,
            DaysUntilServiceStarts = daysUntilServiceStarts,

            Needs = needs.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value),

            Attributes = attributes.ToDictionary(
                kvp => kvp.Key.ToString(),
                kvp => kvp.Value.ToDto()),

            Skills = skills.ToDictionary(
                kvp => kvp.Key.ToString(),
                kvp => kvp.Value.ToDto()),

            PreferredSkill = preferredSkill?.skillName,
            InterestData = interestData?.interestName,

            Activity = activity.ToString(),
            UseNewestEquipment = useNewestEquipment,

            Equipment = equipment?.Select(e => e.ToDto()).ToList() ?? new List<GoodDto>(),

            CurrentCountyImprovement = currentCountyImprovement?.improvementName,
            PassiveResearchItem = passiveResearchItemData?.researchName,
            CurrentResearchItem = currentResearchItemData?.researchName
        };
    }

    public static PopulationData FromDto(PopulationDto dto)
    {
        PopulationData pop = new PopulationData();

        // 🔹 Basic info
        pop.populationId = dto.PopulationId;

        // 🔹 Reconnect FactionData by ID
        if (dto.FactionId.HasValue)
        {
            pop.factionData = SaveManager.Instance.saveGameData.allFactionDataList
                .FirstOrDefault(f => f.factionId == dto.FactionId.Value);
        }

        pop.location = dto.Location;
        pop.lastLocation = dto.LastLocation;
        pop.destination = dto.Destination;

        pop.firstName = dto.FirstName;
        pop.lastName = dto.LastName;
        pop.isMale = dto.IsMale;
        pop.age = dto.Age;

        // 🔹 Personality (convert string to enum)
        if (Enum.TryParse(dto.Personality, out AllEnums.Personality parsedPersonality))
        {
            pop.personality = parsedPersonality;
        }

        pop.isHero = dto.IsHero;

        // 🔹 HeroType (convert string to enum)
        if (Enum.TryParse(dto.HeroType, out AllEnums.HeroType parsedHeroType))
        {
            pop.HeroType = parsedHeroType;
        }

        pop.numberOfSubordinatesWanted = dto.NumberOfSubordinatesWanted;

        // 🔹 Hero Subordinates (IDs → PopulationData references)
        pop.heroSubordinates = new Godot.Collections.Array<PopulationData>();
        foreach (int subId in dto.HeroSubordinates)
        {
            PopulationData subordinate = SaveManager.Instance.saveGameData.allPopulationDataList
                .FirstOrDefault(p => p.populationId == subId);

            if (subordinate != null)
                pop.heroSubordinates.Add(subordinate);
        }

        // 🔹 Perks
        pop.perks = new Godot.Collections.Dictionary<AllEnums.Perks, PerkData>();
        foreach (var kvp in dto.Perks)
        {
            if (Enum.TryParse(kvp.Key, out AllEnums.Perks perkEnum))
            {
                PerkData perk = PerkData.FromDto(kvp.Value);
                pop.perks[perkEnum] = perk;
            }
        }

        // 🔹 Expendables
        pop.hitPoints = dto.HitPoints;
        pop.maxHitPoints = dto.MaxHitPoints;
        pop.moraleExpendable = dto.MoraleExpendable;
        pop.loyaltyBase = dto.LoyaltyBase;
        pop.LoyaltyAdjusted = dto.LoyaltyAdjusted;
        pop.Happiness = dto.Happiness;

        // 🔹 Employment / time info
        pop.daysEmployed = dto.DaysEmployed;
        pop.daysEmployedButIdle = dto.DaysEmployedButIdle;
        pop.daysStarving = dto.DaysStarving;
        pop.daysRecruited = dto.DaysRecruited;
        pop.daysUntilServiceStarts = dto.DaysUntilServiceStarts;

        // 🔹 Needs (string → enum)
        pop.needs = new Godot.Collections.Dictionary<AllEnums.CountyGoodType, int>();
        foreach (var kvp in dto.Needs)
        {
            if (Enum.TryParse(kvp.Key, out AllEnums.CountyGoodType goodType))
            {
                pop.needs[goodType] = kvp.Value;
            }
        }

        // 🔹 Attributes
        pop.attributes = new Godot.Collections.Dictionary<AllEnums.Attributes, AttributeData>();
        foreach (var kvp in dto.Attributes)
        {
            if (Enum.TryParse(kvp.Key, out AllEnums.Attributes attrEnum))
            {
                pop.attributes[attrEnum] = AttributeData.FromDto(kvp.Value);
            }
        }

        // 🔹 Skills
        pop.skills = new Godot.Collections.Dictionary<AllEnums.Skills, SkillData>();
        foreach (var kvp in dto.Skills)
        {
            if (Enum.TryParse(kvp.Key, out AllEnums.Skills skillEnum))
            {
                pop.skills[skillEnum] = SkillData.FromDto(kvp.Value);
            }
        }

        // 🔹 Preferred Skill (by name)
        if (!string.IsNullOrEmpty(dto.PreferredSkill))
        {
            pop.preferredSkill = Autoload.Instance.allSkillData
                .FirstOrDefault(s => s.skillName == dto.PreferredSkill);
        }

        // 🔹 InterestData (by name)
        if (!string.IsNullOrEmpty(dto.InterestData))
        {
            pop.interestData = Autoload.Instance.allInterestData
                .FirstOrDefault(i => i.interestName == dto.InterestData);
        }

        // 🔹 Activity (convert string to enum)
        if (Enum.TryParse(dto.Activity, out AllEnums.Activities parsedActivity))
        {
            pop.activity = parsedActivity;
        }

        pop.useNewestEquipment = dto.UseNewestEquipment;

        // 🔹 Equipment (convert GoodDto → GoodData)
        if (dto.Equipment != null)
        {
            pop.equipment = dto.Equipment.Select(g => GoodData.FromDto(g)).ToArray();
        }

        // 🔹 Current County Improvement
        if (!string.IsNullOrEmpty(dto.CurrentCountyImprovement))
        {
            pop.currentCountyImprovement = Autoload.Instance.allCountyImprovementData
                .FirstOrDefault(c => c.improvementName == dto.CurrentCountyImprovement);
        }

        // 🔹 Passive Research
        if (!string.IsNullOrEmpty(dto.PassiveResearchItem))
        {
            pop.passiveResearchItemData = Autoload.Instance.allResearchItemData
                .FirstOrDefault(r => r.researchName == dto.PassiveResearchItem);
        }

        // 🔹 Current Research
        if (!string.IsNullOrEmpty(dto.CurrentResearchItem))
        {
            pop.currentResearchItemData = Autoload.Instance.allResearchItemData
                .FirstOrDefault(r => r.researchName == dto.CurrentResearchItem);
        }

        return pop;
    }

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
        HeroType = HeroType == AllEnums.HeroType.FactionLeader
            ? AllEnums.HeroType.FactionLeaderArmyLeader
            : AllEnums.HeroType.ArmyLeader;

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
        string name = TranslationServer.Translate(Autoload.Instance.allActivityData[(int)activity].name);

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
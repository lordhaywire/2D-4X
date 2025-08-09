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
    [Export] public int factionId;
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
    [Export] public Godot.Collections.Array<PopulationData> heroSubordinates;

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
    public Godot.Collections.Dictionary<AllEnums.Attributes, AttributeData> attributes;

    [ExportGroup("Skills")] [Export] public Godot.Collections.Dictionary<AllEnums.Skills, SkillData> skills;
    [Export] public SkillData preferredSkill;
    [Export] public InterestData interestData;

    [ExportGroup("Work")] [Export] public AllEnums.Activities activity;

    [ExportGroup("Inventory")] [Export] public bool useNewestEquipment;
    [Export] public Godot.Collections.Dictionary<AllEnums.InventorySlot, GoodData> inventory;

    [Export] public CountyImprovementData currentCountyImprovement; // Used for work and building.

    [Export] public ResearchItemData passiveResearchItemData;
    [Export] public ResearchItemData currentResearchItemData;

    public HeroToken heroToken;

    public PopulationDto ToDto()
    {
        // ✅ Convert Inventory (Dictionary<InventorySlot, GoodData>) → Dictionary<InventorySlot, GoodDto>
        // Convert Dictionary<InventorySlot, GoodData> → Dictionary<string, GoodDto>
        Dictionary<string, GoodDto> inventoryDtos =
            inventory != null
                ? inventory.ToDictionary(
                    kvp => kvp.Key.ToString(), // 🔹 Convert enum to string
                    kvp => kvp.Value?.ToDto()) // 🔹 Convert GoodData → GoodDto (handles null safely)
                : new Dictionary<string, GoodDto>();


        return new PopulationDto
        {
            PopulationId = populationId,
            FactionId = factionId,
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

            Inventory = inventoryDtos,

            CurrentCountyImprovement = currentCountyImprovement?.improvementName,
            PassiveResearchItem = passiveResearchItemData?.researchName,
            CurrentResearchItem = currentResearchItemData?.researchName
        };
    }

    public static PopulationData FromDto(PopulationDto populationDto)
    {
        PopulationData populationData = new PopulationData();

        // Basic info
        populationData.populationId = populationDto.PopulationId;

        // Todo: We need to reconnect this after allFactionDataList is loaded from disk.
        
        /*
        // Reconnect FactionData by ID
        if (populationDto.FactionId.HasValue)
        {
            populationData.factionData = SaveManager.Instance.saveGameData.allFactionDataList
                .FirstOrDefault(f => f.factionId == populationDto.FactionId.Value);
        }
        */

        populationData.location = populationDto.Location;
        populationData.lastLocation = populationDto.LastLocation;
        populationData.destination = populationDto.Destination;

        populationData.firstName = populationDto.FirstName;
        populationData.lastName = populationDto.LastName;
        populationData.isMale = populationDto.IsMale;
        populationData.age = populationDto.Age;

        // 🔹 Personality (convert string to enum)
        if (Enum.TryParse(populationDto.Personality, out AllEnums.Personality parsedPersonality))
        {
            populationData.personality = parsedPersonality;
        }
        // Todo We need to assign the actual personality here.
        
        populationData.isHero = populationDto.IsHero;

        // 🔹 HeroType (convert string to enum)
        if (Enum.TryParse(populationDto.HeroType, out AllEnums.HeroType parsedHeroType))
        {
            populationData.HeroType = parsedHeroType;
        }

        populationData.numberOfSubordinatesWanted = populationDto.NumberOfSubordinatesWanted;

        // Todo: This needs to happen after the allPopulationDataList is fully loaded.
        // 🔹 Hero Subordinates (IDs → PopulationData references)
        populationData.heroSubordinates = new Godot.Collections.Array<PopulationData>();
        foreach (int subId in populationDto.HeroSubordinates)
        {
            PopulationData subordinate = SaveManager.Instance.saveGameData.allPopulationDataList
                .FirstOrDefault(p => p.populationId == subId);

            if (subordinate != null)
                populationData.heroSubordinates.Add(subordinate);
        }

        // 🔹 Perks
        populationData.perks = new Godot.Collections.Dictionary<AllEnums.Perks, PerkData>();
        foreach (KeyValuePair<string, PerkDto> keyValuePair in populationDto.Perks)
        {
            if (Enum.TryParse(keyValuePair.Key, out AllEnums.Perks perkEnum))
            {
                PerkData perk = PerkData.FromDto(keyValuePair.Value);
                populationData.perks[perkEnum] = perk;
            }
        }

        // 🔹 Expendables
        populationData.hitPoints = populationDto.HitPoints;
        populationData.maxHitPoints = populationDto.MaxHitPoints;
        populationData.moraleExpendable = populationDto.MoraleExpendable;
        populationData.loyaltyBase = populationDto.LoyaltyBase;
        populationData.LoyaltyAdjusted = populationDto.LoyaltyAdjusted;
        populationData.Happiness = populationDto.Happiness;

        // 🔹 Employment / time info
        populationData.daysEmployed = populationDto.DaysEmployed;
        populationData.daysEmployedButIdle = populationDto.DaysEmployedButIdle;
        populationData.daysStarving = populationDto.DaysStarving;
        populationData.daysRecruited = populationDto.DaysRecruited;
        populationData.daysUntilServiceStarts = populationDto.DaysUntilServiceStarts;

        // 🔹 Needs (string → enum)
        populationData.needs = new Godot.Collections.Dictionary<AllEnums.CountyGoodType, int>();
        foreach (KeyValuePair<string, int> keyValuePair in populationDto.Needs)
        {
            if (Enum.TryParse(keyValuePair.Key, out AllEnums.CountyGoodType goodType))
            {
                populationData.needs[goodType] = keyValuePair.Value;
            }
        }

        // 🔹 Attributes
        populationData.attributes = new Godot.Collections.Dictionary<AllEnums.Attributes, AttributeData>();
        foreach (KeyValuePair<string, AttributeDto> keyValuePair in populationDto.Attributes)
        {
            if (Enum.TryParse(keyValuePair.Key, out AllEnums.Attributes attrEnum))
            {
                populationData.attributes[attrEnum] = AttributeData.FromDto(keyValuePair.Value);
            }
        }

        // 🔹 Skills
        populationData.skills = new Godot.Collections.Dictionary<AllEnums.Skills, SkillData>();
        foreach (KeyValuePair<string, SkillDto> keyValuePair in populationDto.Skills)
        {
            if (Enum.TryParse(keyValuePair.Key, out AllEnums.Skills skillEnum))
            {
                populationData.skills[skillEnum] = SkillData.FromDto(keyValuePair.Value);
            }
        }

        // 🔹 Preferred Skill (by name)
        if (!string.IsNullOrEmpty(populationDto.PreferredSkill))
        {
            populationData.preferredSkill = Autoload.Instance.allSkillData
                .FirstOrDefault(s => s.skillName == populationDto.PreferredSkill);
        }

        // 🔹 InterestData (by name)
        if (!string.IsNullOrEmpty(populationDto.InterestData))
        {
            populationData.interestData = Autoload.Instance.allInterestData
                .FirstOrDefault(i => i.interestName == populationDto.InterestData);
        }

        // 🔹 Activity (convert string to enum)
        if (Enum.TryParse(populationDto.Activity, out AllEnums.Activities parsedActivity))
        {
            populationData.activity = parsedActivity;
        }

        populationData.useNewestEquipment = populationDto.UseNewestEquipment;

        // Convert Dictionary<string, GoodDto> → Dictionary<InventorySlot, GoodData>
        Godot.Collections.Dictionary<AllEnums.InventorySlot, GoodData> inventoryResult = new Godot.Collections.Dictionary<AllEnums.InventorySlot, GoodData>();

        if (populationDto.Inventory != null)
        {
            foreach (KeyValuePair<string, GoodDto> keyValuePair in populationDto.Inventory)
            {
                // 🔹 Convert the string key back into the InventorySlot enum
                AllEnums.InventorySlot slot = Enum.Parse<AllEnums.InventorySlot>(keyValuePair.Key);

                // 🔹 If the GoodDto is not null, convert it to GoodData
                GoodData goodData = null;
                if (keyValuePair.Value != null)
                {
                    goodData = GoodData.FromDto(keyValuePair.Value);
                }

                // 🔹 Add the converted key/value to the dictionary
                inventoryResult[slot] = goodData;
            }
        }
        else
        {
            // 🔹 No inventory provided → initialize an empty dictionary
            inventoryResult = new Godot.Collections.Dictionary<AllEnums.InventorySlot, GoodData>();
        }

        // 🔹 Assign the finished dictionary to the population data object
        populationData.inventory = inventoryResult;

        // Current County Improvement
        if (!string.IsNullOrEmpty(populationDto.CurrentCountyImprovement))
        {
            populationData.currentCountyImprovement = Autoload.Instance.allCountyImprovementData
                .FirstOrDefault(c => c.improvementName == populationDto.CurrentCountyImprovement);
        }

        // Passive Research
        if (!string.IsNullOrEmpty(populationDto.PassiveResearchItem))
        {
            populationData.passiveResearchItemData = Autoload.Instance.allResearchItemData
                .FirstOrDefault(r => r.researchName == populationDto.PassiveResearchItem);
        }

        // Current Research
        if (!string.IsNullOrEmpty(populationDto.CurrentResearchItem))
        {
            populationData.currentResearchItemData = Autoload.Instance.allResearchItemData
                .FirstOrDefault(r => r.researchName == populationDto.CurrentResearchItem);
        }

        return populationData;
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
        FactionData factionData = SaveManager.Instance.saveGameData.ConvertFactionIdToFactionData(factionId);
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
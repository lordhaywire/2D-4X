using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace;

public partial class PopulationGeneration : Node
{
    private readonly Random random = new();

    private Node2D countiesParent;
    private County county;
    private CountyData countyData;

    private string firstName;
    private string lastName;
    private bool isMale;

    [ExportGroup("For Population Generation")]
    [Export] private int startingAttributeMin = 21;
    [Export] private int startingAttributeMax = 81; // One above max.
    [Export] private int startingSkillMin = 1;
    [Export] private int startingSkillMax = 51; // One above max.
    [Export] private int chanceOfBeingUnhelpful = 11; // One above max.

    // This has to be up here, so the other methods can access it for Preferred Skill.
    private Godot.Collections.Dictionary<AllEnums.Attributes, AttributeData> newAttributes = [];
    private SkillData preferredSkill;

    public override void _Ready()
    {
        CreateFactionLeaders();
        CreatePopulation();
        AssignPlayerSpecificThings();
    }

    // Eventually this will assign other things, but currently we just need this here.
    private static void AssignPlayerSpecificThings()
    {
        Globals.Instance.playerFactionData.factionLeader.personality = AllEnums.Personality.Player;
    }

    private void CreateFactionLeaders()
    {
        foreach (FactionData factionData in Globals.Instance.allFactionData)
        {
            countiesParent = Globals.Instance.countiesParent;
            // Generate Faction Leader County Population
            //GD.Print($"{factionData.factionName} Capital ID: {factionData.factionCapitalCounty}");
            county = (County)countiesParent.GetChild(factionData.factionCapitalCounty);
            countyData = county.countyData;
            GeneratePopulation(true, 1); // There is never going to be more than 1 faction leader.

            factionData.factionLeader = county.countyData.heroesInCountyList[0];
            countyData.population += countyData.heroesInCountyList.Count;
        }
    }

    // ChatGPT wrote this.
    private static int GeneratePersonalities()
    {
        int randomPersonality;
        do
        {
            randomPersonality = (int)AllEnums.GetRandomEnumValue<AllEnums.Personality>();
        } while (randomPersonality == 0);

        //GD.Print("Random Personality: " + randomPersonality);
        return randomPersonality;
    }

    private void GeneratePopulation(bool hero, int totalPopulation)
    {
        for (int i = 0; i < totalPopulation; i++)
        {
            GenerateNameAndSex();  // This could probably be broken into two methods.
            // This needs to be up here because the personality is needed for iPersonality.
            AllEnums.Personality newPersonality = (AllEnums.Personality)GeneratePersonalities();
            int loyaltyBase = random.Next(31, 101); // This is a temporary number.
            
            if (hero == false)
            {
                // This is for the standard population.
                PopulationData newPopulationData = new()
                {
                    factionData = countyData.factionData,
                    location = countyData.countyId,
                    lastLocation = -1,
                    destination = -1,
                    firstName = firstName,
                    lastName = lastName,
                    isMale = isMale,
                    age = GenerateAge(),
                    personality = newPersonality,
                    iPersonality = AllEnums.AssignPersonalityInterfaces(newPersonality),
                    isHero = false,
                    //isWorker = false,
                    HeroType = AllEnums.HeroType.None,
                    heroSubordinates = [],
                    perks = GeneratePopulationPerks(),
                    hitPoints = Globals.Instance.startingHitPoints,
                    maxHitPoints = Globals.Instance.startingHitPoints,
                    moraleExpendable = GenerateExpendables(),
                    loyaltyBase = loyaltyBase,
                    LoyaltyAdjusted = loyaltyBase,
                    happiness = random.Next(31, 101),
                    daysStarving = 0,
                    daysUntilServiceStarts = -1,
                    needs = GenerateNeeds(),
                    attributes = GenerateAttributes(),
                    skills = GenerateSkillsList(),
                    preferredSkill = preferredSkill,
                    interestData = GenerateInterest(),
                    activity = AllEnums.Activities.Idle,
                    useNewestEquipment = false,
                    equipment = new GoodData[5],
                    currentCountyImprovement = null,
                    passiveResearchItemData = null,
                    currentResearchItemData = null,
                    heroToken = null,
                };
                countyData.populationDataList.Add(newPopulationData);
            }
            else
            {
                PopulationData newPopulationData = new()
                {
                    factionData = countyData.factionData,
                    location = countyData.countyId,
                    lastLocation = -1,
                    destination = -1,
                    firstName = firstName,
                    lastName = lastName,
                    isMale = isMale,
                    age = GenerateAge(),
                    personality = newPersonality,
                    iPersonality = AllEnums.AssignPersonalityInterfaces(newPersonality),
                    isHero = true,
                    //isWorker = false, // Why is this true?
                    HeroType = AllEnums.HeroType.FactionLeader,
                    numberOfSubordinatesWanted = 0,
                    heroSubordinates = [],
                    perks = GenerateLeaderPerks(),
                    hitPoints = Globals.Instance.startingHitPoints,
                    maxHitPoints = Globals.Instance.startingHitPoints,
                    moraleExpendable = GenerateExpendables(),
                    loyaltyBase = loyaltyBase,
                    LoyaltyAdjusted = loyaltyBase,
                    happiness = random.Next(31, 101), // Our leaders can be unhappy?
                    Happiness = 0,
                    daysEmployed = 0,
                    daysEmployedButIdle = 0,
                    daysStarving = 0,
                    daysUntilServiceStarts = -1,
                    needs = GenerateNeeds(),
                    attributes = GenerateAttributes(),
                    skills = GenerateSkillsList(),
                    preferredSkill = preferredSkill,
                    interestData = GenerateInterest(),
                    activity = AllEnums.Activities.Idle,
                    useNewestEquipment = false,
                    equipment = new GoodData[5],
                    currentCountyImprovement = null,
                    passiveResearchItemData = null,
                    currentResearchItemData = null,
                    heroToken = null,
                };
                // Add the hero to the county hero's list.
                countyData.heroesInCountyList.Add(newPopulationData);
                // Add the hero to allHeroesList
                countyData.factionData.AddHeroToAllHeroesList(newPopulationData);
            }
                
        }
    }
    private static InterestData GenerateInterest()
    {
        InterestData interest = AllInterests.Instance.GetRandomInterest();
        //GD.Print("Interest: " + interest.name);
        return interest;
    }
    private static Godot.Collections.Dictionary<AllEnums.CountyGoodType, int> GenerateNeeds()
    {
        Godot.Collections.Dictionary<AllEnums.CountyGoodType, int> needs = [];
        needs.Add(AllEnums.CountyGoodType.Remnants, 75);
        return needs;
    }

    private void GeneratePreferredWork(Godot.Collections.Dictionary<AllEnums.Skills, SkillData> skills)
    {
        Rolls rolls = new();
        IEnumerable<KeyValuePair<AllEnums.Skills, SkillData>> sortedSkills
            = skills.Where(keyValue => !keyValue.Value.isCombatSkill).OrderByDescending(keyValue => keyValue.Value.skillLevel);

        // Get the skill with the highest skill level
        SkillData possiblePreferredSkill = sortedSkills.First().Value;
        preferredSkill = possiblePreferredSkill;

        // Roll an Intelligence check, and if it passes, then the top skill is the preferred skill.
        // If the intelligence check fails, it is ok if the random roll randomly assigns the same preferred skill.
        if (rolls.Attribute(newAttributes[AllEnums.Attributes.Intelligence])) return;
        {
            // Create a list of non-combat skills for random selection
            List<AllEnums.Skills> nonCombatSkills = [.. skills
                .Where(keyValue => !keyValue.Value.isCombatSkill)
                .Select(keyValue => keyValue.Key)];

            if (nonCombatSkills.Count <= 0) return;
            int randomIndex = random.Next(0, nonCombatSkills.Count);
            AllEnums.Skills randomSkill = nonCombatSkills[randomIndex];
            preferredSkill = skills[randomSkill];
        }
    }

    /// <summary>
    /// This also generates preferred work.
    /// </summary>
    /// <returns></returns>
    private Godot.Collections.Dictionary<AllEnums.Skills, SkillData> GenerateSkillsList()
    {
        Godot.Collections.Dictionary<AllEnums.Skills, SkillData> newSkills = [];

        foreach (SkillData skillData in AllSkills.Instance.allSkillData)
        {
            newSkills.Add(skillData.skill, (SkillData)skillData.Duplicate());
            newSkills[skillData.skill].skillLevel = random.Next(startingSkillMin, startingSkillMax);
        }
        GeneratePreferredWork(newSkills);
        return newSkills;
    }

    private Godot.Collections.Dictionary<AllEnums.Attributes, AttributeData> GenerateAttributes()
    {
        newAttributes = new(AttributeData.NewCopy());
        foreach (KeyValuePair<AllEnums.Attributes, AttributeData> keyValuePair in newAttributes)
        {
            newAttributes[keyValuePair.Key].attributeLevel = random.Next(startingAttributeMin, startingAttributeMax);
        }
        //GD.PrintRich($"[rainbow]Intelligence: {newAttributes[AllEnums.Attributes.Intelligence].attributeLevel}");
        return newAttributes;
    }

    // This doesn't seem that useful.
    private static int GenerateExpendables()
    {
        int moraleExpendable = 100;
        return moraleExpendable;
    }

    private int GenerateAge()
    {
        // Determine the person's age.
        int age = random.Next(18, 61);
        return age;
    }

    private void GenerateNameAndSex()
    {
        // Generates Persons Last Name
        List<string> lastNames = Globals.Instance.lastNames;
        List<string> femaleNames = Globals.Instance.femaleNames;
        List<string> maleNames = Globals.Instance.maleNames;

        int randomLastNameNumber = random.Next(0, lastNames.Count);
        lastName = lastNames[randomLastNameNumber];

        // Determine the persons sex and first name
        int randomSexNumber = random.Next(0, 2);
        int randomFemaleNameNumber = random.Next(0, femaleNames.Count);
        int randomMaleNameNumber = random.Next(0, maleNames.Count);

        if (randomSexNumber == 0)
        {
            isMale = true;
            firstName = maleNames[randomMaleNameNumber];
        }
        else
        {
            isMale = false;
            firstName = femaleNames[randomFemaleNameNumber];
        }
    }

    private static Godot.Collections.Dictionary<AllEnums.Perks, PerkData> GenerateLeaderPerks()
    {
        Godot.Collections.Dictionary<AllEnums.Perks, PerkData> perks = [];
        perks.Add(AllEnums.Perks.LeaderOfPeople, AllPerks.Instance.allPerks[(int)AllEnums.Perks.LeaderOfPeople]);
        return perks;
    }
    private Godot.Collections.Dictionary<AllEnums.Perks, PerkData> GeneratePopulationPerks()
    {
        Godot.Collections.Dictionary<AllEnums.Perks, PerkData> perks = [];
        int unhelpfulRoll = random.Next(1, 101);
        if (unhelpfulRoll < chanceOfBeingUnhelpful)
        {
            perks.Add(AllEnums.Perks.Unhelpful, AllPerks.Instance.allPerks[(int)AllEnums.Perks.Unhelpful]);
        }
        return perks;
    }

    private void CreatePopulation()
    {
        countiesParent = Globals.Instance.countiesParent;
        // Create various county specific data.
        for (int i = 0; i < countiesParent.GetChildCount(); i++)
        {
            county = (County)countiesParent.GetChild(i);
            countyData = county.countyData;
            countyData.countyId = i; // Generate countyID.
            //GD.PrintRich("[rainbow]County ID: " + countyData.countyID);

            // Generate the general population for the player and AI Capitals.
            if (countyData.isPlayerCapital || countyData.isAiCapital)
            {
                // Generate Normal Population
                GeneratePopulation(false, Globals.Instance.totalCapitolPop);
                countyData.population += countyData.populationDataList.Count;
                countyData.IdleWorkers = countyData.population -= countyData.heroesInCountyList.Count;
            }
            else
            {
                // Generate Normal Population
                int normalPopulation = random.Next(Globals.Instance.minimumCountyPop, Globals.Instance.maximumCountyPop);
                GeneratePopulation(false, normalPopulation);
                countyData.population += countyData.populationDataList.Count;
                countyData.IdleWorkers = countyData.population -= countyData.heroesInCountyList.Count;
            }
        }
    }
}
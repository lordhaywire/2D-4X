using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace
{
    public partial class CountyPopulationGeneration : Node
    {
        private readonly Random random = new();

        public Node2D countiesParent;
        private County selectCounty;
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

        // This has to be up here so the other methods can access it for Preferred Skill.
        Godot.Collections.Dictionary<AllEnums.Attributes, AttributeData> newAttributes = [];
        private SkillData preferredSkill;
        public override void _Ready()
        {
            CreateFactionLeaders();
            CreatePopulation();
        }

        public void CreateFactionLeaders()
        {
            foreach (FactionData factionData in Globals.Instance.factionDatas)
            {
                countiesParent = Globals.Instance.countiesParent;
                // Generate Faction Leader County Population
                //GD.Print($"{factionData.factionName} Capital ID: {factionData.factionCapitalCounty}");
                selectCounty = (County)countiesParent.GetChild(factionData.factionCapitalCounty);
                countyData = selectCounty.countyData;
                GeneratePopulation(true, 1); // There is never going to be more then 1 faction leader.
                factionData.factionLeader = selectCounty.countyData.herosInCountyList[0];
                countyData.population += countyData.herosInCountyList.Count();
            }
        }

        private void GeneratePopulation(bool hero, int totalPopulation)
        {
            for (int i = 0; i < totalPopulation; i++)
            {
                GenerateNameAndSex();  // This could probably be broken into two methods.
                int loyaltyBase = random.Next(31, 101); // This is a temporary number.
                int happiness = random.Next(31, 101); // This is a temporary number.
                int daysStarving = 0;
                int maxHitpoints = Globals.Instance.startingHitPoints;
                if (hero == false)
                {
                    // This is for the standard population.
                    countyData.countyPopulationList.Add(new CountyPopulation(countyData.factionData, countyData.countyId
                        , -1, -1, firstName, lastName, isMale, GenerateAge(), false, false, false, false, false
                        , GeneratePopulationPerks(), Globals.Instance.startingHitPoints, maxHitpoints
                        , GenerateExpendables()
                        , loyaltyBase, loyaltyBase, happiness, daysStarving, GenerateNeeds()
                        , GenerateAttributes()
                        , GenerateSkillsList()
                        , preferredSkill
                        , GenerateInterest()
                        , AllEnums.Activities.Idle, null, null));
                }
                else
                {
                    // This is adding the Faction Leader.
                    countyData.herosInCountyList.Add(new CountyPopulation(countyData.factionData, countyData.countyId
                        , -1, -1, firstName, lastName, isMale, GenerateAge(), true, true, false, false, false
                        , GenerateLeaderPerks(), Globals.Instance.startingHitPoints, maxHitpoints
                        , GenerateExpendables()
                        , loyaltyBase, loyaltyBase, happiness, daysStarving, GenerateNeeds()
                        , GenerateAttributes()
                        , GenerateSkillsList()
                        , preferredSkill
                        , GenerateInterest()
                        , AllEnums.Activities.Idle, null, null));
                }
            }
        }

        private static InterestData GenerateInterest()
        {
            InterestData interest = AllInterests.Instance.GetRandomInterest();
            GD.Print("Interest: " + interest.name);
            return interest;
        }
        private static Godot.Collections.Dictionary<AllEnums.CountyResourceType, int> GenerateNeeds()
        {
            Godot.Collections.Dictionary<AllEnums.CountyResourceType, int> needs = [];
            needs.Add(AllEnums.CountyResourceType.Remnants, 75);
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

            // Roll an Intelligence check and if it passes then the top skill is the preferred skill.
            // If the intelligence check fails, it is ok if the random roll randomly assigns the same preferred skill.
            if (rolls.Attribute(newAttributes[AllEnums.Attributes.Intelligence]) == false)
            {
                // Create a list of non-combat skills for random selection
                List<AllEnums.Skills> nonCombatSkills = skills
                    .Where(keyValue => !keyValue.Value.isCombatSkill)
                    .Select(keyValue => keyValue.Key)
                    .ToList();

                if (nonCombatSkills.Count > 0)
                {
                    int randomIndex = random.Next(0, nonCombatSkills.Count);
                    AllEnums.Skills randomSkill = nonCombatSkills[randomIndex];
                    preferredSkill = skills[randomSkill];
                }
            }
        }

        private Godot.Collections.Dictionary<AllEnums.Skills, SkillData> GenerateSkillsList()
        {
            Godot.Collections.Dictionary<AllEnums.Skills, SkillData> newSkills = [];

            foreach (SkillData skillData in AllSkills.Instance.allSkills)
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
                selectCounty = (County)countiesParent.GetChild(i);
                countyData = selectCounty.countyData;
                countyData.countyId = i; // Generate countyID.
                //GD.PrintRich("[rainbow]County ID: " + countyData.countyID);

                // Generate the general population for the player Capitals.
                if (countyData.isPlayerCapital == true || countyData.isAICapital)
                {

                    // Generate Hero Population
                    /*
                    GeneratePopulation(true, Globals.Instance.heroPopulation);
                    countyData.population += countyData.heroCountyPopulation.Count;
                    */
                    // Generate Normal Population
                    GeneratePopulation(false, Globals.Instance.totalCapitolPop);
                    countyData.population += countyData.countyPopulationList.Count();
                    countyData.IdleWorkers = countyData.population -= countyData.herosInCountyList.Count();
                }
                else
                {
                    // Generate Hero Population
                    /*
                    GeneratePopulation(true, Globals.Instance.heroPopulation);
                    countyData.population -= countyData.heroCountyPopulation.Count;
                    */

                    // Generate Normal Population
                    int normalPopulation = random.Next(Globals.Instance.minimumCountyPop, Globals.Instance.maximumCountyPop);
                    GeneratePopulation(false, normalPopulation);
                    countyData.population += countyData.countyPopulationList.Count();
                    countyData.IdleWorkers = countyData.population -= countyData.herosInCountyList.Count();
                }
            }
        }
    }
}
using Godot;
using System;
using System.Collections.Generic;

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
        private int age;
        private bool leaderOfPeoplePerk = false;

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
                GD.Print($"{factionData.factionName} Capital ID: {factionData.factionCapitalCounty}");
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

                // Determine the person's age.
                age = random.Next(18, 61);

                // Generate random stats for each population.
                int moraleExpendable = 100;

                int ps = random.Next(20, 81);
                int ag = random.Next(20, 81);
                int en = random.Next(20, 81);
                int ie = random.Next(20, 81);
                int ms = random.Next(20, 81);
                int aw = random.Next(20, 81);
                int ch = random.Next(20, 81);
                int lo = random.Next(20, 81);

                int loyaltyAttribute = 100; // This is a temporary number.

                SkillData constructionSkill = new()
                {
                    skillName = "Construction",
                    skillLevel = random.Next(20, 81),
                    amountLearned = 0,
                    isCombatSkill = false,
                    skillType = AllEnums.SkillType.PhysicalStrength,
                };
                SkillData coolSkill = new()
                {
                    skillName = "Cool",
                    skillLevel = random.Next(20, 81),
                    amountLearned = 0,
                    isCombatSkill = true,
                    skillType = AllEnums.SkillType.MentalStrength,
                }; 
                SkillData researchSkill = new()
                {
                    skillName = "Research",
                    skillLevel = random.Next(20, 81),
                    amountLearned = 0,
                    isCombatSkill = false,
                    skillType = AllEnums.SkillType.Intelligence,
                };
                SkillData rifleSkill = new()
                {
                    skillName = "Rifle",
                    skillLevel = random.Next(20, 81),
                    amountLearned = 0,
                    isCombatSkill = true,
                    skillType = AllEnums.SkillType.Agility,
                };

                if (hero == false)
                {
                    // This adds to the C# list.
                    countyData.countyPopulationList.Add(new CountyPopulation(countyData.factionData, countyData.countyId
                        , -1, -1, firstName, lastName, isMale, age, false, false, false, false, false, false
                        , moraleExpendable, ps, ag,en, ie, ms, aw, ch, lo, loyaltyAttribute, constructionSkill
                        , coolSkill,  researchSkill, rifleSkill, AllText.Activities.IDLE, null
                        , AllText.Activities.IDLE, null, null));
                }
                else
                {
                    // This adds to a C# list.
                    countyData.herosInCountyList.Add(new CountyPopulation(countyData.factionData, countyData.countyId
                        , -1, -1, firstName, lastName, isMale, age, true, true, false, false, false, true
                        , moraleExpendable, ps, ag, en, ie, ms, aw, ch, lo, loyaltyAttribute, constructionSkill
                        , coolSkill, researchSkill, rifleSkill, AllText.Activities.IDLE, null
                        , AllText.Activities.IDLE, null, null));
                }
            }

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
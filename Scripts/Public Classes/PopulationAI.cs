using Godot;
namespace PlayerSpace;

public class PopulationAI
{
    // This is a dumb name for this method.
    public static void IsThereEnoughFood(CountyData countyData)
    {
        int amountOfFood = countyData.CountFactionResourceOfType(AllEnums.FactionResourceType.Food);
        int amountOfPeople = Banker.CountEveryoneInCounty(countyData);
        if (amountOfFood >= (Globals.Instance.foodToGainHappiness * amountOfPeople))
        {
            // Happiness is addedin the PopulationEatsFood method.
            countyData.PopulationEatsFood(countyData.herosInCountyList,
                Globals.Instance.foodToGainHappiness);
            countyData.PopulationEatsFood(countyData.armiesInCountyList,
                Globals.Instance.foodToGainHappiness);
            countyData.PopulationEatsFood(countyData.countyPopulationList,
                Globals.Instance.foodToGainHappiness);
            // Visiting heroes need to be able to eat food too.  It needs to improve faction relations.
        }
        else if (amountOfFood >= (Globals.Instance.foodToGainNothing * amountOfPeople))
        {
            //GD.Print("People get jack shit for happiness.");
            // Happiness is added in the PopulationEatsFood method.
            countyData.PopulationEatsFood(countyData.herosInCountyList,
                Globals.Instance.foodToGainNothing);
            countyData.PopulationEatsFood(countyData.armiesInCountyList,
                Globals.Instance.foodToGainNothing);
            countyData.PopulationEatsFood(countyData.countyPopulationList,
                Globals.Instance.foodToGainNothing);
            // Visiting heroes need to be able to eat food too.  It needs to improve faction relations.

        }
        else if (amountOfFood >= amountOfPeople)
        {
            // Happiness is removed in method PopulationEatsFood.
            countyData.PopulationEatsFood(countyData.herosInCountyList,
                Globals.Instance.foodToLoseHappiness);
            countyData.PopulationEatsFood(countyData.armiesInCountyList,
                Globals.Instance.foodToLoseHappiness);
            countyData.PopulationEatsFood(countyData.countyPopulationList,
                Globals.Instance.foodToLoseHappiness);
            // Visiting heroes need to be able to eat food too.  It needs to improve faction relations.
        }
        else
        {
            //GD.PrintRich($"[rainbow]People are starting to starve!!");
            // Eating the last of the food, then major penalty for starvation.
            // It will already reduce their happiness by 1 in the Population Eats Food method.
            countyData.PopulationEatsFood(countyData.herosInCountyList,
                Globals.Instance.foodToLoseHappiness);
            countyData.PopulationEatsFood(countyData.armiesInCountyList,
                Globals.Instance.foodToLoseHappiness);
            countyData.PopulationEatsFood(countyData.countyPopulationList,
                Globals.Instance.foodToLoseHappiness);
            // Visiting heroes need to be able to eat food too.  It needs to improve faction relations.
        }
    }

    public static void WorkDayOverForPopulation(CountyData countyData)
    {
        foreach (CountyPopulation countyPopulation in countyData.countyPopulationList)
        {
            /*
            if (countyData.factionData.isPlayer == true)
            {
                GD.PrintRich($"[color=blue]{Clock.Instance.GetDateAndTime()} {countyPopulation.firstName} {countyPopulation.activity}[/color]");
            }
            */
            switch (countyPopulation.activity)
            {
                case AllEnums.Activities.Scavenge:
                    //GD.Print($"{countyPopulation.firstName} {countyPopulation.lastName} is generating scavenged resources.");
                    // Skill learning is done in the GenerateScavengedResources.
                    Banker.GenerateScavengedResources(countyData, countyPopulation);
                    countyPopulation.UpdateActivity(AllEnums.Activities.Idle);
                    break;
                case AllEnums.Activities.Build:
                    // Skill learning is done in the CompleteConstructionWithSkilLCheck
                    CompleteConstructionWithSkillCheck(countyPopulation);
                    // If the building is done they will be set to idle somewhere else.
                    break;
                case AllEnums.Activities.Work:
                    // Produce resources based on the countyimprovement
                    Banker.Work(countyData, countyPopulation);

                    // Check for Skill Learning.
                    SkillData.CheckLearning(countyPopulation
                        , countyPopulation.skills[countyPopulation.currentCountyImprovement.workSkill]
                        , AllEnums.LearningSpeed.slow);
                    // Check loyalty to see if they still want to work there and if they don't then they
                    // get set to idle.
                    KeepWorkingAtCountyImprovement(countyPopulation);
                    break;
                case AllEnums.Activities.Research:
                    // Person working at research office generates research.
                    // Check learning is done in Banker.AddResearchForOfficeResearch
                    Banker.AddResearchForOfficeResearch(countyPopulation);

                    KeepResearching(countyPopulation);
                    break;
                case AllEnums.Activities.Idle:
                    // Give idle people their bonus happiness.
                    countyPopulation.AddRandomHappiness(5);
                    break;
            }
        }
        GD.PrintRich($"[rainbow]{countyData.countyName}: Work Day Over For Population.");
    }

    /// <summary>
    /// Checks loyalty and if the research is done.
    /// </summary>
    /// <param name="countyPopulation"></param>
    private static void KeepResearching(CountyPopulation countyPopulation)
    {
        ResearchItemData researchItemData = countyPopulation.currentResearchItemData;
        // Check for loyalty
        if (CheckLoyaltyWithSkillCheck(countyPopulation) == false)
        {
            // Remove From Research needs to be above Remove From County Improvement, so that the remove
            // from county improvement sets them to idle.
            countyPopulation.RemoveFromResearch();
            countyPopulation.RemoveFromCountyImprovement();
            GD.Print($"{countyPopulation.firstName} is failed the loyalty check.");
            return;
        }

        // Check if the research is finished.
        if (researchItemData?.isResearchDone == true)
        {
            EventLog.Instance.AddLog($"{researchItemData.researchName} has been completed.");
            GD.Print($"{countyPopulation.firstName} has finished the research.");
            countyPopulation.RemoveFromResearch();
        }
    }
    private static void KeepWorkingAtCountyImprovement(CountyPopulation countyPopulation)
    {
        if (CheckLoyaltyWithSkillCheck(countyPopulation) == false)
        {
            countyPopulation.RemoveFromCountyImprovement();
        }
    }
    private static void CompleteConstructionWithSkillCheck(CountyPopulation countyPopulation)
    {
        if (SkillData.Check(countyPopulation, countyPopulation.skills[AllEnums.Skills.Construction].skillLevel
            , countyPopulation.skills[AllEnums.Skills.Construction].attribute, false))
        {
            countyPopulation.currentCountyImprovement.CurrentAmountOfConstruction
                += Globals.Instance.dailyConstructionAmount + Globals.Instance.dailyConstructionAmountBonus;
        }
        else
        {
            countyPopulation.currentCountyImprovement.CurrentAmountOfConstruction
                += Globals.Instance.dailyConstructionAmount;
        }
        SkillData.CheckLearning(countyPopulation, countyPopulation.skills[AllEnums.Skills.Construction]
            , AllEnums.LearningSpeed.slow);
    }


    // This should be moved to the Resource that Loyalty will be part of once we figure out
    // what catagory Loyalty is.  For example, it isn't a skill, or a perk.
    private static bool CheckLoyaltyWithSkillCheck(CountyPopulation countyPopulation)
    {
        if (SkillData.Check(countyPopulation, countyPopulation.LoyaltyAdjusted, AllEnums.Attributes.MentalStrength, false))
        {
            return true;
        }
        return false;
    }
}
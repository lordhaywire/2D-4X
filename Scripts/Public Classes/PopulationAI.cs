namespace PlayerSpace;

public class PopulationAI
{
    // This is a dumb name for this method.
    public static void IsThereEnoughFood(CountyData countyData)
    {
        int amountOfFood = countyData.CountFactionResourceOfType(AllEnums.FactionGoodType.Food);
        int amountOfPeople = Banker.CountEveryoneInCounty(countyData);
        if (amountOfFood >= (Globals.Instance.foodToGainHappiness * amountOfPeople))
        {
            // Happiness is addedin the PopulationEatsFood method.
            countyData.PopulationEatsFood(countyData.heroesInCountyList,
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
            countyData.PopulationEatsFood(countyData.heroesInCountyList,
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
            countyData.PopulationEatsFood(countyData.heroesInCountyList,
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
            countyData.PopulationEatsFood(countyData.heroesInCountyList,
                Globals.Instance.foodToLoseHappiness);
            countyData.PopulationEatsFood(countyData.armiesInCountyList,
                Globals.Instance.foodToLoseHappiness);
            countyData.PopulationEatsFood(countyData.countyPopulationList,
                Globals.Instance.foodToLoseHappiness);
            // Visiting heroes need to be able to eat food too.  It needs to improve faction relations.
        }
    }

    
    /// <summary>
    /// Checks loyalty and if the research is done.
    /// </summary>
    /// <param name="countyPopulation"></param>
    public static void KeepResearching(CountyPopulation countyPopulation)
    {
        ResearchItemData researchItemData = countyPopulation.currentResearchItemData;
        // Check for loyalty
        if (CheckLoyaltyWithSkillCheck(countyPopulation) == false)
        {
            // Remove From Research needs to be above Remove From County Improvement, so that the remove
            // from county improvement sets them to idle.
            countyPopulation.RemoveFromResearch();
            countyPopulation.RemoveFromCountyImprovement();
            // GD.Print($"{countyPopulation.firstName} is failed the loyalty check.");
            return;
        }

        // Check if the research is finished.
        if (researchItemData?.CheckIfResearchDone() == true)
        {
            // GD.Print($"{countyPopulation.firstName} has finished the research.");
            countyPopulation.RemoveFromResearch();
        }
    }
    public static void LoyaltyCheckToKeepWorkingAtCountyImprovement(CountyPopulation countyPopulation)
    {
        if (CheckLoyaltyWithSkillCheck(countyPopulation) == false)
        {
            countyPopulation.RemoveFromCountyImprovement();
        }
    }
    

    // This is no longer used, I don't think.  Left here temporarily just in case.
    /*
    public static void CompleteConstructionWithSkillCheck(CountyPopulation countyPopulation)
    {
        if (SkillData.Check(countyPopulation, countyPopulation.skills[AllEnums.Skills.Construction].skillLevel
            , countyPopulation.skills[AllEnums.Skills.Construction].attribute, false))
        {
            countyPopulation.currentCountyImprovement.CurrentAmountOfConstruction
                += Globals.Instance.dailyWorkAmount + Globals.Instance.dailyWorkAmountBonus;
        }
        else
        {
            countyPopulation.currentCountyImprovement.CurrentAmountOfConstruction
                += Globals.Instance.dailyWorkAmount;
        }
        SkillData.CheckLearning(countyPopulation
            , AllEnums.LearningSpeed.slow);
    }
    */

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
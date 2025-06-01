using Godot;

namespace PlayerSpace;

public abstract class Explorer
{
    /// <summary>
    /// The scouting hero will get a bonus to the skill roll for each subordinate that passes a skill check.
    /// The subordinate does not add work to the exploration.  They only give the hero a bonus.
    /// </summary>
    /// <param name="hero"></param>
    /// <param name="countyData"></param>
    public static void ExploreCounty(CountyData countyData, PopulationData hero)
    {
        // Possibly remove this.
        /*
        if (countyData.explorationEvents.Count <= 0)
        {
            hero.UpdateActivity(AllEnums.Activities.Idle);
            CountyInfoControl.Instance.UpdateEverything();
            GD.PrintRich($"It got to the part it shouldn't have.");
            return;
        }
        */

        int skillLevel = hero.skills[AllEnums.Skills.Scout].skillLevel;
        int attributeLevel = hero.attributes[hero.skills[AllEnums.Skills.Scout].attribute].attributeLevel;
        int attributeBonus = AttributeData.GetAttributeBonus(attributeLevel, false, false);
        int additionalBonus =
            GetSubordinateBonus(
                hero); // TODO: Add Scouting Equipment Bonus.  Possibly add subordinate scouting equipment bonus too.
        int perkBonus = 0; //Add a perk of some sort. // TODO: Perk Bonus
        int exploredAmount = 0;
        // Only gives exploration progress if the skill check passes.
        if (SkillData.CheckWithBonuses(skillLevel, attributeBonus, additionalBonus, perkBonus))
        {
            exploredAmount = Globals.Instance.dailyWorkAmount;
            EventLog.Instance.AddLog(
                $"{hero.GetFullName()} {TranslationServer.Translate("PHRASE_HAS_SUCCESSFULLY_EXPLORED")} {countyData.countyName}");
        }
        // The first event on the list of random events.
        countyData.explorationEvents[0].amountExplored += exploredAmount;
        
        CheckForFinishedExploration(countyData, hero);
    }

    private static int GetSubordinateBonus(PopulationData hero)
    {
        int bonusToHeroAmount = 0;
        foreach (PopulationData subordinate in hero.heroSubordinates)
        {
            int skillLevel = subordinate.skills[AllEnums.Skills.Scout].skillLevel;
            int attributeLevel = subordinate.attributes[subordinate.skills[AllEnums.Skills.Scout].attribute]
                .attributeLevel;
            int attributeBonus = AttributeData.GetAttributeBonus(attributeLevel, false, false);
            int additionalBonus = 0;
            int perkBonus = 0; //Add a perk of some sort.// TODO: Perk Bonus

            if (SkillData.CheckWithBonuses(skillLevel, attributeBonus, additionalBonus, perkBonus))
            {
                bonusToHeroAmount += Globals.Instance.subordinateExplorationBonus;
            }
        }

        return bonusToHeroAmount;
    }

    private static void CheckForFinishedExploration(CountyData countyData, PopulationData hero)
    {
        if (countyData.explorationEvents[0].amountExplored < Globals.Instance.explorationCost)
        {
            return;
        }

        StoryEventControl.Instance.currentStoryEventData = countyData.explorationEvents[0];
        StoryEventControl.Instance.Show();
        countyData.explorationEvents.RemoveAt(0);
        
        // Check to see if all the exploration is done and if it is set the hero to idle.
        if (countyData.explorationEvents.Count > 0) return;
        hero.UpdateActivity(AllEnums.Activities.Idle);
        CountyInfoControl.Instance.UpdateEverything();
    }
}
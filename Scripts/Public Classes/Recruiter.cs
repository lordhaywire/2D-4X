using Godot;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace;

public static class Recruiter
{
    public static void CheckForRecruitment(CountyData countyData)
    {
        // If the hero isn't moving then recruit.
        // Adds the army list to the end of the hero list and then checks both of them.
        foreach (PopulationData populationData in countyData.heroesInCountyList.Concat(countyData.armiesInCountyList)
            .Where(populationData => populationData.activity != AllEnums.Activities.Move))
        {
            // Not enough recruits, so start recruiting.
            if (populationData.heroSubordinates.Count < populationData.numberOfSubordinatesWanted)
            {
                GD.Print("Number of subordinates is less then the number wanted, so recruiting has started.");
                populationData.UpdateActivity(AllEnums.Activities.Recruit);
                int numberOfSubordinatesToHire = populationData.numberOfSubordinatesWanted - populationData.heroSubordinates.Count;

                RecruitSubordinates(populationData, numberOfSubordinatesToHire);
            }
            else if (populationData.heroSubordinates.Count > populationData.numberOfSubordinatesWanted)
            {
                GD.Print("Number of subordinates is greater then the number wanted, so firing has started.");
                int numberOfSubordinatesToFire = populationData.heroSubordinates.Count - populationData.numberOfSubordinatesWanted;
                FireSubordinates(populationData, numberOfSubordinatesToFire);
                GD.Print("Number of Subordinates in the Hero Subordinates list: " + populationData.heroSubordinates.Count);
            }
            else
            {
                GD.Print("Number of subordinates does match, so nothing is being done.");
            }
        }
    }

    private static void RecruitSubordinates(PopulationData populationData, int numberOfSubordinatesToHire)
    {
        County county = (County)Globals.Instance.countiesParent.GetChild(populationData.location);
        CountyData countyData = county.countyData;

        // Filter out those with LoyaltyAdjusted below the threshold, if they are already in the list,
        // and those who have the "Unhelpful" perk.
        List<PopulationData> eligibleSubordinates = [.. countyData.populationDataList
            .Where(person => person.LoyaltyAdjusted >= Globals.Instance.willFightLoyalty
                && !person.perks.ContainsKey(AllEnums.Perks.Unhelpful)
                && !populationData.heroSubordinates.Contains(person))
                .OrderByDescending(person => person.LoyaltyAdjusted)
                .Take(numberOfSubordinatesToHire)];
        PopulationData recruitee = eligibleSubordinates.FirstOrDefault();
        GD.PrintRich($"Recruitee: " + recruitee?.firstName);
        if (recruitee != null)
        {
            int attributeLevel = populationData.attributes[populationData.skills[AllEnums.Skills.Leadership].attribute].attributeLevel;
            GD.PrintRich($"[rainbow]Attribute Level: " + attributeLevel);
            int attributeBonus = AttributeData.GetAttributeBonus(attributeLevel, false, false);
            GD.PrintRich($"[rainbow]Attribute Bonus: " + attributeBonus);
            int additionalBonus = AttributeData.GetAttributeBonus(recruitee.LoyaltyAdjusted, false, false);
            GD.PrintRich($"[rainbow]Additional Bonus: " + additionalBonus);
            GD.PrintRich($"[rainbow]Leader of People Perk bonus: " + populationData.perks[AllEnums.Perks.LeaderOfPeople].perkBonus);
            // Hero needs to do a leadership roll with an attribute bonus of the recruitee's loyalty bonus.
            bool skillCheck = SkillData.CheckWithBonuses(populationData.skills[AllEnums.Skills.Leadership].skillLevel
                , attributeBonus
                , additionalBonus
                , populationData.perks[AllEnums.Perks.LeaderOfPeople].perkBonus);
            GD.PrintRich($"[rainbow]Recruit Skill Check: " + skillCheck);
            // Person has been recruited. Random number of days before service starts will be generated.
            if (skillCheck)
            {
                GD.PrintRich($"[rainbow]Recruitee added: " + recruitee.firstName);
                recruitee.daysUntilServiceStarts = Globals.Instance.random.Next(1, Globals.Instance.maxDaysUntilServiceStarts);
                populationData.heroSubordinates.Add(recruitee);
                EventLog.Instance.AddLog($"{countyData.countyName}: {populationData.GetFullName()} " +
                    $"{TranslationServer.Translate("WORD_RECRUITED")} {recruitee.GetFullName()}.");
            }
        }
        else
        {
            // Recruiter needs to stop recruiting when there are no more people available for recruitment.
            EventLog.Instance.AddLog($"{countyData.countyName}: {TranslationServer.Translate("PHRASE_NO_MORE_PEOPLE_TO_RECRUIT")}");
        }

        // Change recruitee's activity to Service once the number of days to service starts is done.

        // If the hero is unsuccessful then they try again the next day.  They try 3 times before they move on to the next person.
        // Maybe have some sort of loyalty reduction on each try.
    }

    public static void CheckForRecruitingActivity(PopulationData populationData)
    {
        GD.Print($"Hero Subordinate Count: {populationData.heroSubordinates.Count} vs {populationData.numberOfSubordinatesWanted}");

        populationData.UpdateActivity(populationData.heroSubordinates.Count < populationData.numberOfSubordinatesWanted
            ? AllEnums.Activities.Recruit
            : AllEnums.Activities.Idle);
    }
    /// <summary>
    /// We currently could get the numberToFire from the populationData, but I bet we can use this
    /// method at other times.
    /// </summary>
    /// <param name="populationData"></param>
    /// <param name="numberToFire"></param>
    private static void FireSubordinates(PopulationData populationData, int numberToFire)
    {
        Godot.Collections.Array<PopulationData> peopleToFireSorted
            = [.. populationData.heroSubordinates.ToList().OrderBy(personData
            => personData.LoyaltyAdjusted).Skip(numberToFire)];
        populationData.heroSubordinates = peopleToFireSorted;
    }
}

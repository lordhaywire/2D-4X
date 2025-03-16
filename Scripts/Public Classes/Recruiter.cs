using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace;

public class Recruiter
{
    public static void CheckForRecruitment(CountyData countyData)
    {
        foreach (PopulationData populationData in countyData.heroesInCountyList)
        {
            // Not enough recruits, so start recruiting.
            if (populationData.heroSubordinates.Count < populationData.numberOfSubordinates)
            {
                GD.Print("Number of subordinates is less then the number wanted, so recruiting has started.");
                populationData.UpdateActivity(AllEnums.Activities.Recruit);
                int numberOfSubordinatesToHire = populationData.numberOfSubordinates - populationData.heroSubordinates.Count;

                HireSubordinates(populationData, numberOfSubordinatesToHire);
            }
            else if (populationData.heroSubordinates.Count > populationData.numberOfSubordinates)
            {
                GD.Print("Number of subordinates is greater then the number wanted, so firing has started.");
                int numberOfSubordinatesToFire = populationData.heroSubordinates.Count - populationData.numberOfSubordinates;
                FireSubordinates(populationData, numberOfSubordinatesToFire);
                GD.Print("Number of Subordinates in the Hero Suborbinates list: " + populationData.heroSubordinates.Count);
            }
            else
            {
                GD.Print("Number of subordinates does match, so nothing is being done.");
            }
        }
    }

    private static void HireSubordinates(PopulationData populationData, int numberOfSubordinatesToHire)
    {
        County county = (County)Globals.Instance.countiesParent.GetChild(populationData.location);
        CountyData countyData = county.countyData;

        // Filter out those with LoyaltyAdjusted below the threshold and those who have the "Unhelpful" perk.
        List<PopulationData> eligibleSubordinates = [.. countyData.populationDataList
            .Where(person => person.LoyaltyAdjusted >= Globals.Instance.willFightLoyalty
                && !person.perks.ContainsKey(AllEnums.Perks.Unhelpful)
                && !populationData.heroSubordinates.Contains(person))
                .OrderByDescending(person => person.LoyaltyAdjusted)
                .Take(numberOfSubordinatesToHire)];

        PopulationData recruitee = eligibleSubordinates.FirstOrDefault();
        // Hero needs to do a leadership roll with an attribute bonus of the recruitee's loyalty bonus.
        bool skillCheck = SkillData.Check(populationData, populationData.skills[AllEnums.Skills.Leadership].skillLevel
            , populationData.skills[AllEnums.Skills.Leadership].attribute, false);

        // Person has been recruited. Random number of days before service starts will be generated.
        if (skillCheck)
        {
            recruitee.daysUntilServiceStarts = Globals.Instance.random.Next(1, Globals.Instance.maxDaysUntilServiceStarts);
            populationData.heroSubordinates.Add(recruitee);
        }


        // Change recruitee's activity to Service once the numberofdaystoservicestarts is done.

        // If the hero is unsuccussful then they try again the next day.  They try 3 times before they move on to the next person.
        // Maybe have some sort of loyalty reduction on each try.

        // Convert to a Godot.Collections.Array if necessary.
        populationData.heroSubordinates = [.. eligibleSubordinates];
    }

    /*

    private static void HireSubordinates(PopulationData populationData, int numberOfSubordinatesToHire)
    {
        County county = (County)Globals.Instance.countiesParent.GetChild(populationData.location);
        CountyData countyData = county.countyData;

        Godot.Collections.Array<PopulationData> peopleToHireSorted
            = [.. countyData.populationDataList.ToList().OrderByDescending(populationData
            => populationData.LoyaltyAdjusted).Take(numberOfSubordinatesToHire)];
        
        populationData.heroSubordinates = peopleToHireSorted;
    }
    */

    /// <summary>
    /// We currently could get the numberToFire from the populationData, but I bet we can use this
    /// method at other times.
    /// </summary>
    /// <param name="populationData"></param>
    /// <param name="numberToFire"></param>
    public static void FireSubordinates(PopulationData populationData, int numberToFire)
    {
        Godot.Collections.Array<PopulationData> peopleToFireSorted
            = [.. populationData.heroSubordinates.ToList().OrderBy(populationData
            => populationData.LoyaltyAdjusted).Skip(numberToFire)];
        populationData.heroSubordinates = peopleToFireSorted;
    }
}

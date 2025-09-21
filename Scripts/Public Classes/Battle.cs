using System;
using AutoloadSpace;

namespace PlayerSpace;

public class Battle(CountyData battleLocation)
{
    public readonly CountyData battleLocation = battleLocation;

    public static int GetAverageArmyMorale(PopulationData heroPopulationData)
    {
        int morale = 0;
        foreach (PopulationData subordinateData in heroPopulationData.heroSubordinates)
        {
            morale += subordinateData.moraleExpendable; 
        }

        morale += heroPopulationData.moraleExpendable;
        int averageMorale = morale / (heroPopulationData.heroSubordinates.Count + 1);
        return averageMorale;
    }

    public static int GenerateCombatDamage()
    {
        Random random = new();
        return random.Next(1, Autoload.Instance.startingHitPoints + 1);
    }
}
using Godot;
using System;
using System.Collections.Generic;

namespace PlayerSpace;
public class Haulmaster
{
    // Gathers stockpiled goods for the county improvement based on its input needs.
    public static void GatherStockpileGoods(CountyData countyData, CountyImprovementData countyImprovementData)
    {
        // Mark the improvement as currently producing.
        countyImprovementData.status = AllEnums.CountyImprovementStatus.Producing;

        foreach (KeyValuePair<GoodData, int> inputGood in countyImprovementData.inputGoods)
        {
            // Calculate the desired stockpile range.
            int minStockpileAmount = inputGood.Value * countyImprovementData.adjustedMaxWorkers 
                * Globals.Instance.minDaysStockpile;
            int maxStockpileAmount = inputGood.Value * countyImprovementData.adjustedMaxWorkers 
                * Globals.Instance.maxDaysStockpile;

            // Get the county's stockpile and available amount for the required good.
            GoodData countyGood = countyData.goods[inputGood.Key.countyGoodType];

            GD.Print($"{countyImprovementData.improvementName} requires: {inputGood.Key.goodName}, " +
                     $"Available: {countyGood.Amount}, " +
                     $"Stockpiled: {countyImprovementData.countyStockpiledGoods[countyGood.countyGoodType]}");

            // Skip if the current stockpile meets or exceeds the maximum desired amount.
            if (countyImprovementData.countyStockpiledGoods[countyGood.countyGoodType] 
                >= maxStockpileAmount)
            {
                continue;
            }

            // Determine how much to transfer from the county's stock to the improvement's stockpile.
            int amountToStockpile = Math.Min(minStockpileAmount, countyGood.Amount);

            // Transfer goods and update stockpile.
            if (amountToStockpile > 0)
            {
                countyGood.Amount -= amountToStockpile;
                countyImprovementData.countyStockpiledGoods[countyGood.countyGoodType] += amountToStockpile;
            }

            // If the stockpile is below the minimum desired amount, update the improvement status and
            // add a day to each person's employed but idle count.
            if (countyImprovementData.countyStockpiledGoods[countyGood.countyGoodType] 
                < minStockpileAmount)
            {
                countyImprovementData.status = AllEnums.CountyImprovementStatus.LowStockpiledGoods;
            }

            // Log the post-transfer state.
            GD.Print($"Updated {countyImprovementData.improvementName} stockpile for {inputGood.Key.goodName}: " +
                     $"Available: {countyGood.Amount}, " +
                     $"Stockpiled: {countyImprovementData.countyStockpiledGoods[countyGood.countyGoodType]} " +
                     $"Min Stockpiled: {minStockpileAmount} " + 
                     $"Status: {countyImprovementData.status}");
        }
    }

    public static void DeductStockPiledGoods(CountyImprovementData countyImprovementData)
    {
        // Deduct input goods and perform work actions.
        foreach (KeyValuePair<GoodData, int> inputGood in countyImprovementData.inputGoods)
        {
            countyImprovementData.countyStockpiledGoods[inputGood.Key.countyGoodType]
                -= inputGood.Value;
        }
    }
    /*
    // Multiply the input goods by the adjusted max number of workers then multiple by the stockpile multiplier.
    public static void GatherStockpileGoods(CountyData countyData, CountyImprovementData countyImprovementData)
    {
        countyImprovementData.status = AllEnums.CountyImprovementStatus.Producing;
        foreach (KeyValuePair<GoodData, int> keyValuePair in countyImprovementData.inputGoods)
        {
            int minStockpileAmountWanted = keyValuePair.Value * countyImprovementData.adjustedMaxWorkers
                * Globals.Instance.minDaysStockpile;
            int maxStockpileAmountWanted = keyValuePair.Value * countyImprovementData.adjustedMaxWorkers
                * Globals.Instance.maxDaysStockpile;

            GoodData countyGood = countyData.goods[keyValuePair.Key.countyGoodType];
            int amountStockpiled = countyImprovementData.countyStockpiledGoods[countyGood.countyGoodType];

            // The county improvement needs to get as many of the good as they can.

            GD.Print($"Input good : {countyImprovementData.improvementName} " +
            $": {keyValuePair.Key.goodName} : County Amount {countyGood.Amount} : Stockpiled Amount " +
            $"{amountStockpiled}");


            // If the county improvement has over the max amount then it skips to the next good it needs.
            if (amountStockpiled >= maxStockpileAmountWanted)
            {
                continue;
            }

            // If the goods wanted to stockpile is greater then 2 days (currently) it will take 2 days worth.
            if (countyGood.Amount >= minStockpileAmountWanted)
            {
                countyGood.Amount -= minStockpileAmountWanted;
                countyImprovementData.countyStockpiledGoods[countyGood.countyGoodType] 
                    += minStockpileAmountWanted;
            }
            else
            {
                countyImprovementData.countyStockpiledGoods[countyGood.countyGoodType] 
                    += countyGood.Amount;
                countyGood.Amount = 0;
            }

            if(amountStockpiled < minStockpileAmountWanted)
            {
                countyImprovementData.status = AllEnums.CountyImprovementStatus.LowStockpiledGoods;
            }
            GD.Print($"Input good : {countyImprovementData.improvementName} " +
                $": {keyValuePair.Key.goodName} : {countyGood.Amount} : " +
                $"{amountStockpiled}");
        }
    }
    */
    /*
    /// <summary>
    /// It sets the countyimprovementStatus to low stockpiled good, and if there is any good that is
    /// greater then 0 it sets it back to producing.
    /// </summary>
    /// <param name="countyImprovementData"></param>
    /// <param name="stockpiledAmount"></param>
    private static void CheckIfLowStockpiledGoods(CountyImprovementData countyImprovementData
        , int goodNeededPerDay, int stockpiledAmount)
    {
        countyImprovementData.status = AllEnums.CountyImprovementStatus.LowStockpiledGoods;
        int neededGoods = Globals.Instance.lowStockpiledGoodDays * countyImprovementData.adjustedMaxWorkers
            * goodNeededPerDay;
        if (stockpiledAmount > neededGoods)
        {
            countyImprovementData.status = AllEnums.CountyImprovementStatus.Producing;
        }
    }
    */
    public static void GenerateStockpileGoodsDictionary(CountyImprovementData countyImprovementData)
    {
        foreach (KeyValuePair<GoodData, int> keyValuePair in countyImprovementData.inputGoods)
        {
            countyImprovementData.countyStockpiledGoods[keyValuePair.Key.countyGoodType] = 0;
        }
    }
}

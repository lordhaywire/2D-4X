using Godot;
using System;
using System.Collections.Generic;

namespace PlayerSpace;
public class Haulmaster
{
    // Multiply the input goods by the adjusted max number of workers then multiple by the stockpile multiplier.
    public static void GatherStockpileGoods(CountyData countyData, CountyImprovementData countyImprovementData)
    {
        foreach (KeyValuePair<GoodData, int> keyValuePair in countyImprovementData.inputGoods)
        {
            int stockpileAmountWanted = keyValuePair.Value * countyImprovementData.adjustedMaxWorkers 
                * Globals.Instance.inputStockpileMultiplier;
            GoodData countyGood = countyData.goods[keyValuePair.Key.countyGoodType];
            // The county improvement needs to get as many of the good as they can so it loops through
            // and does that.

            GD.Print($"Input good : {countyImprovementData.improvementName} " +
            $": {keyValuePair.Key.goodName} : {countyGood.Amount} : " +
            $"{countyImprovementData.countyStockpiledGoods[keyValuePair.Key.countyGoodType]}");
            if (stockpileAmountWanted <= countyGood.Amount)
            {
                countyGood.Amount -= stockpileAmountWanted;
                countyImprovementData.countyStockpiledGoods[countyGood.countyGoodType] += stockpileAmountWanted;
            }
            else
            {
                countyImprovementData.countyStockpiledGoods[countyGood.countyGoodType] += countyGood.Amount;
                countyGood.Amount = 0;
            }
            GD.Print($"Input good : {countyImprovementData.improvementName} " +
                $": {keyValuePair.Key.goodName} : {countyGood.Amount} : " +
                $"{countyImprovementData.countyStockpiledGoods[countyGood.countyGoodType]}");

            CheckIfNoGoodsStockpiled(countyImprovementData, countyImprovementData.countyStockpiledGoods[countyGood.countyGoodType]);
        }
    }

    /// <summary>
    /// It sets the countyimprovementStatus to Not enough stockpiled good, and if there is any good that is
    /// greater then 0 it sets it back to producing.
    /// </summary>
    /// <param name="countyImprovementData"></param>
    /// <param name="stockpiledAmount"></param>
    private static void CheckIfNoGoodsStockpiled(CountyImprovementData countyImprovementData, int stockpiledAmount)
    {
        countyImprovementData.status = AllEnums.CountyImprovementStatus.NotEnoughStockpiledGoods;
        if (stockpiledAmount > 0)
        {
            countyImprovementData.status = AllEnums.CountyImprovementStatus.Producing;
        }
    }

    public static void GenerateStockpileGoodsDictionary(CountyImprovementData countyImprovementData)
    {
        foreach (KeyValuePair<GoodData, int> keyValuePair in countyImprovementData.inputGoods)
        {
            countyImprovementData.countyStockpiledGoods[keyValuePair.Key.countyGoodType] = 0;
        }
    }
}

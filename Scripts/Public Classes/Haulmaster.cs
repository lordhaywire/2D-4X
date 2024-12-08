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

            for (int i = 0; i < stockpileAmountWanted; i++) 
            {
                if (countyGood.Amount > stockpileAmountWanted)
                {
                    countyGood.Amount -= keyValuePair.Value;
                    countyImprovementData.stockpiledGoods[countyGood] += keyValuePair.Value;
                }
                else
                {
                    return;
                }
            }
        }
    }

    public static void GenerateStockpileGoodsDictionary(CountyImprovementData countyImprovementData)
    {
        foreach (KeyValuePair<GoodData, int> keyValuePair in countyImprovementData.inputGoods)
        {
            countyImprovementData.stockpiledGoods[keyValuePair.Key] = 0;
        }
    }
}

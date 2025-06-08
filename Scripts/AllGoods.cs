using System.Collections.Generic;
using System.Linq;
using Godot;

namespace PlayerSpace;

public partial class AllGoods : Node
{
    public static AllGoods Instance { get; private set; }

    public List<GoodData> allGoods = [];
    private string goodsDirectory = "res://Resources/Goods/";

    public override void _Ready()
    {
        Instance = this;
        allGoods = Globals.Instance.ReadResourcesFromDisk(goodsDirectory).Cast<GoodData>().ToList();
        CountGoods();
    }

    /// <summary>
    /// Since there is an enum of None (which is zero when parsed to an int), we need to subtract
    /// 1 from the CountyGoodType when getting the good with the AllEnums from the allGoods
    /// array.
    /// </summary>
    /// <param name="goodType"></param>
    /// <returns></returns>
    public GoodData GetCorrectGoodData(AllEnums.CountyGoodType goodType)
    {
        GoodData correctGoodData = allGoods[(int)goodType - 1];
        return correctGoodData;
    }
    private void CountGoods()
    {
        int perishable = 0;
        int nonperishable = 0;

        foreach (GoodData resourceData in allGoods)
        {
            switch (resourceData.perishable)
            {
                case AllEnums.Perishable.Perishable:
                    perishable++;
                    break;
                case AllEnums.Perishable.Nonperishable:
                    nonperishable++;
                    break;
            }
        }
        Globals.Instance.numberOfPerishableGoods = perishable;
        Globals.Instance.numberOfNonperishableGoods = nonperishable;
    }
}
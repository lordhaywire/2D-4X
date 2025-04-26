using Godot;

namespace PlayerSpace;

public partial class AllGoods : Node
{
    public static AllGoods Instance { get; private set; }

    [Export] public GoodData[] allGoods;

    public override void _Ready()
    {
        Instance = this;
        CountGoods();
    }

    /// <summary>
    /// Since there is an enum of None (which is zero when parsed to an int), we need to subtract
    /// 1 from the CountyResourceType when getting the resource with the AllEnums from the allResources
    /// array.
    /// </summary>
    /// <param name="resourceType"></param>
    /// <returns></returns>
    public GoodData GetCountyResourceData(AllEnums.CountyGoodType resourceType)
    {
        GoodData countyResourceData = allGoods[(int)resourceType - 1];
        return countyResourceData;
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
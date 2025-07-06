using Godot;
using AutoloadSpace;

namespace PlayerSpace;

public partial class AllGoods : Node
{
    //public static AllGoods Instance { get; private set; }

    //public List<GoodData> allGoods = [];
    //private string goodsDirectory = "res://Resources/Goods/";

    public override void _Ready()
    {
        //Instance = this;
        //allGoods = Autoload.Instance.ReadResourcesFromDisk(goodsDirectory).Cast<GoodData>().ToList();
        //CountGoods();
    }


    private void CountGoods()
    {
        int perishable = 0;
        int nonperishable = 0;

        foreach (GoodData resourceData in Autoload.Instance.allGoods)
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
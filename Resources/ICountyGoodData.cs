using Godot;
namespace PlayerSpace
{
    [GlobalClass]
    public partial class ICountyGoodData : Resource, IGoodsData
    {
        [Export] public string GoodName { get; set; }
    }
}
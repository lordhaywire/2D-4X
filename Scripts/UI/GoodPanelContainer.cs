using Godot;

namespace PlayerSpace;
public partial class GoodPanelContainer : PanelContainer
{
    [Export] public Label goodLabel;
    [Export] public CheckBox useRemnantsCheckBox;
    // We aren't using this right now, but it is quite possible we will use it in the future.
    // This would be used for if the county improvement has multiple goods it produces, and we want to let the player
    // select only some goods to produce.
    [Export] public CheckBox onlyProduceCheckBox;
}

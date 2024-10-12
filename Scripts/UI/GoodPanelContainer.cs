using Godot;

namespace PlayerSpace;
public partial class GoodPanelContainer : PanelContainer
{
    [Export] public Label goodLabel;
    [Export] public CheckBox useRemnantsCheckBox;
    // We aren't using this right now, but it is quite possible we will use it in the future.
    [Export] public CheckBox onlyProduceCheckBox;
}

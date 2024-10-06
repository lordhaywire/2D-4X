using Godot;

namespace PlayerSpace;
public partial class GoodPanelContainer : PanelContainer
{
    [Export] public Label goodLabel;
    [Export] public CheckBox useRemnantsCheckBox;
    [Export] public CheckBox onlyProduceCheckBox;
}

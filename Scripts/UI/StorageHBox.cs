using Godot;

namespace PlayerSpace;

public partial class StorageHBox : HBoxContainer
{
    [Export] public GoodData goodData;
    [Export] public Label goodNameLabel;
    [Export] public Label goodAmountLabel;

    [Export] public SpinBox maxAmountSpinBox;

    private static void ValueChanged(float value)
    {
        //GD.Print("Value Changed:" + value);
        GoodsPanelContainer.Instance.UpdateCountyAvailableStorageLabels();
        GoodsPanelContainer.Instance.UpdateSpinBoxMaxValuePlusLabel();
    }
}
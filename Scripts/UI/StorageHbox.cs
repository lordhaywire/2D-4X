using Godot;

namespace PlayerSpace
{
    public partial class StorageHbox : HBoxContainer
    {
        [Export] public ResourceData resourceData;
        [Export] public Label resourceNameLabel;
        [Export] public Label resourceAmountLabel;
        [Export] public Label resourceMaxAmountLabel;

        [Export] public SpinBox maxAmountSpinBox;

        private static void ValueChanged(float value)
        {
            GD.Print("Value Changed:" + value);
            ResourcesPanelContainer.Instance.UpdateCountyAvailableStorageLabels();
            ResourcesPanelContainer.Instance.UpdateSpinBoxMaxValuePlusLabel();
        }
    }
}
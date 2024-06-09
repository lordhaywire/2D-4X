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

        private void ValueChanged(float value)
        {
            ResourcesPanelContainer.Instance.UpdateCountyAvailableStorage();
            ResourcesPanelContainer.Instance.UpdateSpinBoxMaxValue(resourceData.perishable);
        }
    }
}
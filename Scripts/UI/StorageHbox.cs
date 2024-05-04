using Godot;

namespace PlayerSpace
{
    public partial class StorageHbox : HBoxContainer
    {
        [Export] public ResourceData resourceData;
        public int storageHboxIndex;
        [Export] public Label resourceNameLabel;
        [Export] public Label resourceAmountLabel;
        [Export] public Label resourceMaxAmountLabel;

        [Export] public SpinBox maxAmountSpinBox;
        public bool perishable;

        private void ValueChanged(float value)
        {
            ResourcesPanelContainer.Instance.UpdateCountyAvailableStorage(perishable);
            ResourcesPanelContainer.Instance.UpdateSpinBoxMaxValue(perishable);
        }
    }
}
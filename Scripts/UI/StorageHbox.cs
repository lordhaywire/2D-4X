using Godot;

namespace PlayerSpace
{
    public partial class StorageHbox : HBoxContainer
    {
        public int storageHboxIndex;
        [Export] public Label resourceNameLabel;
        [Export] public Label resourceAmountLabel;
        [Export] public Label resourceMaxAmountLabel;

        [Export] public SpinBox maxAmountSpinBox;
        public bool perishable;

        private void ValueChanged(float value)
        {
            ResourcesPanelContainer.Instance.UpdateAvailableStorage((int)value, perishable);
            ResourcesPanelContainer.Instance.UpdateSpinBoxMaxValue((int)value, perishable, storageHboxIndex);
        }
    }
}
using Godot;

namespace PlayerSpace
{
    public partial class PopAndIdleButtons : VBoxContainer
    {
        [Export] private MarginContainer populationListMarginContainer;

        private void PopAndIdleOnButtonPressed(bool isVisitorList)
        {
            Globals.Instance.isVisitorList = isVisitorList;
            populationListMarginContainer.Show();
        }

        private void CloseButton()
        {
            populationListMarginContainer.Hide();
        }
    }
}
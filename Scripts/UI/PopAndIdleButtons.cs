using Godot;
using System.Collections.Generic;

namespace PlayerSpace
{
    public partial class PopAndIdleButtons : VBoxContainer
    {
        [Export] private MarginContainer populationListMarginContainer;

        private void OnButtonClick()
        {
            populationListMarginContainer.Show();
        }

        private void CloseButton()
        {
            populationListMarginContainer.Hide();
        }
    }
}
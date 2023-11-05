using Godot;

namespace PlayerSpace
{
	public partial class RecruitButton : Button
	{
        [Export] private PanelContainer confirmationPanelContainer;

        private void OpenConfirmationPanel()
        {
            confirmationPanelContainer.Show();
        }
    }
}
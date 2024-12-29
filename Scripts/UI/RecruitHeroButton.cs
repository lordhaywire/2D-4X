using Godot;

namespace PlayerSpace
{
	public partial class RecruitHeroButton : Button
	{
        [Export] private RecruitHeroConfirmationPanelContainer recruitHeroConfirmationPanelContainer;

        private void OpenConfirmationPanel(bool armyLeaderRecruited)
        {
            recruitHeroConfirmationPanelContainer.Show();
            recruitHeroConfirmationPanelContainer.armyLeaderRecruited = armyLeaderRecruited;
        }
    }
}
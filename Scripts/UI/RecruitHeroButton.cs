using Godot;

namespace PlayerSpace
{
	public partial class RecruitHeroButton : Button
	{
        [Export] private RecruitHeroConfirmationPanelContainer recruitHeroConfirmationPanelContainer;

        [Export] private Button recruitButton;
        public override void _Ready()
        {
        }

        /// <summary>
        /// This works for both Army Leader recruitment button and Aide recruitment button.
        /// It passes the ArmyLeaderRecruited bool to the confirmation panel.
        /// </summary>
        /// <param name="armyLeaderRecruited"></param>
        private void OpenConfirmationPanel(bool armyLeaderRecruited)
        {
            recruitHeroConfirmationPanelContainer.Show();
            recruitHeroConfirmationPanelContainer.armyLeaderRecruited = armyLeaderRecruited;
        }
    }
}
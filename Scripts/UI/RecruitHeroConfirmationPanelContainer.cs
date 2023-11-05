using Godot;

namespace PlayerSpace
{
    public partial class RecruitHeroConfirmationPanelContainer : PanelContainer
    {
        [Export] private Label titleLabel;
        private void OnVisibilityChange()
        {
            titleLabel.Text = $"{AllText.Titles.RECRUITHEROCONFIRM} {Globals.Instance.selectedCountyPopulation.firstName} {Globals.Instance.selectedCountyPopulation.lastName}";
        }
    }
}
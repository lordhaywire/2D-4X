using Godot;

namespace PlayerSpace
{
    public partial class RecruitHeroConfirmationPanelContainer : PanelContainer
    {
        [Export] private Label titleLabel;
        private void OnVisibilityChange()
        {
            if (Visible == true)
            {
                titleLabel.Text = $"{AllText.Titles.RECRUITHEROCONFIRM} {Globals.Instance.selectedCountyPopulation.firstName} {Globals.Instance.selectedCountyPopulation.lastName}";
            }
        }

        private void YesButton()
        {
            SelectCounty selectCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(Globals.Instance.selectedCountyPopulation.location);

            selectCounty.countyData.countyPopulation.Remove(Globals.Instance.selectedCountyPopulation);
            selectCounty.countyData.heroCountyPopulation.Add(Globals.Instance.selectedCountyPopulation);
            
            CountyInfoControl.Instance.GenerateHeroesPanelList();

            Banker.Instance.ChargeForHero();
            Globals.Instance.selectedCountyPopulation.isHero = true;

            PopulationDescriptionControl.Instance.UpdateDescriptionInfo();

            GD.Print("County Name: " + selectCounty.countyData.countyName + selectCounty.countyData.heroCountyPopulation.Count);
            GD.Print("You clicked the Yes Button");
            Hide();
        }

        private void NoButton()
        {
            GD.Print("You clicked the No Button");
            Hide();
        }
    }
}
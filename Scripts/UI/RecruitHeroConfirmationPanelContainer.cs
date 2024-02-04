using Godot;

namespace PlayerSpace
{
    public partial class RecruitHeroConfirmationPanelContainer : PanelContainer
    {
        [Export] private Label titleLabel;
        [Export] private bool armyLeaderRecruited;
        private void OnVisibilityChange()
        {
            if (Visible == true)
            {
                titleLabel.Text = $"{AllText.Titles.RECRUITHEROCONFIRM} " +
                    $"{PopulationDescriptionControl.Instance.countyPopulation.firstName} " +
                    $"{PopulationDescriptionControl.Instance.countyPopulation.lastName}";
            }
        }
        private void OpenConfirmationPanel(bool armyLeaderRecruited)
        {
            Show();
            GD.Print("Army Leader Recruited? " + armyLeaderRecruited);
            this.armyLeaderRecruited = armyLeaderRecruited;
        }
        private void YesButton()
        {
            CountyPopulation countyPopulation = PopulationDescriptionControl.Instance.countyPopulation;
            SelectCounty selectCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(countyPopulation.location);

            if (countyPopulation.isHero != true)
            {
                selectCounty.countyData.countyPopulation.Remove(countyPopulation);
                selectCounty.countyData.heroCountyPopulation.Add(countyPopulation);
            }

            if(armyLeaderRecruited == false)
            {
                countyPopulation.isHero = true;
                countyPopulation.isAide = true;
            }
            else
            {
                countyPopulation.isHero = true;
                countyPopulation.isAide = false;
                countyPopulation.isArmyLeader = true;
            }

            Banker.Instance.ChargeForHero();
            PopulationDescriptionControl.Instance.UpdateDescriptionInfo();
            CountyInfoControl.Instance.GenerateHeroesPanelList();

            Hide();
        }

        private void NoButton()
        {
            Hide();
        }
    }
}
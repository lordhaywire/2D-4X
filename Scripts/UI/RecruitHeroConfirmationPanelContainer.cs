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
                titleLabel.Text = $"{AllText.Titles.RECRUITHEROCONFIRM} {Globals.Instance.selectedCountyPopulation.firstName} {Globals.Instance.selectedCountyPopulation.lastName}";
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
            SelectCounty selectCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(Globals.Instance.selectedCountyPopulation.location);

            if (Globals.Instance.selectedCountyPopulation.isHero != true)
            {
                selectCounty.countyData.countyPopulation.Remove(Globals.Instance.selectedCountyPopulation);
                selectCounty.countyData.heroCountyPopulation.Add(Globals.Instance.selectedCountyPopulation);
            }

            Banker.Instance.ChargeForHero();
            if(armyLeaderRecruited == false)
            {
                Globals.Instance.selectedCountyPopulation.isHero = true;
                Globals.Instance.selectedCountyPopulation.isAide = true;
            }
            else
            {
                Globals.Instance.selectedCountyPopulation.isHero = true;
                Globals.Instance.selectedCountyPopulation.isAide = false;
                Globals.Instance.selectedCountyPopulation.isArmyLeader = true;
            }

            PopulationDescriptionControl.Instance.UpdateDescriptionInfo();
            CountyInfoControl.Instance.GenerateHeroesPanelList();

            GD.Print("County Name and Hero Population: " + selectCounty.countyData.countyName + selectCounty.countyData.heroCountyPopulation.Count);
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
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
            County selectCounty = (County)Globals.Instance.countiesParent.GetChild(countyPopulation.location);

            if (countyPopulation.isHero != true)
            {
                selectCounty.countyData.countyPopulationList.Remove(countyPopulation);
            }
            if (armyLeaderRecruited == false)
            {
                countyPopulation.isHero = true;
                countyPopulation.isAide = true;
                selectCounty.countyData.herosInCountyList.Add(countyPopulation);
            }
            else
            {
                countyPopulation.isHero = true;
                countyPopulation.isAide = false;
                countyPopulation.IsArmyLeader = true;
                selectCounty.countyData.herosInCountyList.Remove(countyPopulation);
                selectCounty.countyData.armiesInCountyList.Add(countyPopulation);
            }

            // This is set again to update the sprite textures;
            if (countyPopulation.token != null)
            {
                AllTokenTextures.Instance.AssignTokenTextures(countyPopulation.token);
                countyPopulation.token.UpdateSpriteTexture();
                countyPopulation.token.spawnedTokenButton.UpdateButtonIcon();
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
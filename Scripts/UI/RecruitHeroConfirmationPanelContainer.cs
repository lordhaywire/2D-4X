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
                titleLabel.Text = $"{Tr("PHRASE_RECRUIT_HERO_CONFIRMATION")} " +
                    $"{PopulationDescriptionControl.Instance.countyPopulation.firstName} " +
                    $"{PopulationDescriptionControl.Instance.countyPopulation.lastName}";
            }
        }
        private void OpenConfirmationPanel(bool armyLeaderRecruited)
        {
            Show();
            //GD.Print("Army Leader Recruited? " + armyLeaderRecruited);
            this.armyLeaderRecruited = armyLeaderRecruited;
        }
        private void YesButton()
        {
            CountyPopulation countyPopulation = PopulationDescriptionControl.Instance.countyPopulation;
            County county = (County)Globals.Instance.countiesParent.GetChild(countyPopulation.location);

            if (countyPopulation.isHero != true)
            {
                county.countyData.countyPopulationList.Remove(countyPopulation);
            }
            if (armyLeaderRecruited == false)
            {
                countyPopulation.isHero = true;
                countyPopulation.isAide = true;
                county.countyData.heroesInCountyList.Add(countyPopulation);
                county.countyData.factionData.AddHeroToAllHeroesList(countyPopulation);
            }
            else
            {
                countyPopulation.isHero = true;
                countyPopulation.isAide = false;
                countyPopulation.IsArmyLeader = true;
                county.countyData.heroesInCountyList.Remove(countyPopulation);
                county.countyData.armiesInCountyList.Add(countyPopulation);
                county.countyData.factionData.AddHeroToAllHeroesList(countyPopulation);

            }

            // This is set again to update the sprite textures;
            if (countyPopulation.token != null)
            {
                AllTokenTextures.Instance.AssignTokenTextures(countyPopulation.token);
                countyPopulation.token.UpdateSpriteTexture();
                countyPopulation.token.spawnedTokenButton.UpdateButtonIcon();
            }

            Banker.ChargeForHero(Globals.Instance.playerFactionData);
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
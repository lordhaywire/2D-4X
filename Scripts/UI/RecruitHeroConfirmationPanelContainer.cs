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
                    $"{PopulationDescriptionControl.Instance.populationData.firstName} " +
                    $"{PopulationDescriptionControl.Instance.populationData.lastName}";
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
            PopulationData populationData = PopulationDescriptionControl.Instance.populationData;
            County county = (County)Globals.Instance.countiesParent.GetChild(populationData.location);

            if (populationData.isHero != true)
            {
                county.countyData.populationDataList.Remove(populationData);
            }
            if (armyLeaderRecruited == false)
            {
                populationData.isHero = true;
                populationData.isAide = true;
                county.countyData.heroesInCountyList.Add(populationData);
                county.countyData.factionData.AddHeroToAllHeroesList(populationData);
            }
            else
            {
                populationData.isHero = true;
                populationData.isAide = false;
                populationData.IsArmyLeader = true;
                county.countyData.heroesInCountyList.Remove(populationData);
                county.countyData.armiesInCountyList.Add(populationData);
                county.countyData.factionData.AddHeroToAllHeroesList(populationData);

            }

            // This is set again to update the sprite textures;
            if (populationData.token != null)
            {
                AllTokenTextures.Instance.AssignTokenTextures(populationData.token);
                populationData.token.UpdateSpriteTexture();
                populationData.token.spawnedTokenButton.UpdateButtonIcon();
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
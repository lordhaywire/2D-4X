using Godot;

namespace PlayerSpace;

public partial class RecruitHeroConfirmationPanelContainer : PanelContainer
{
    [Export] private Label titleLabel;
    [Export] public bool armyLeaderRecruited;
    private void OnVisibilityChange()
    {
        if (Visible == true)
        {
            titleLabel.Text = $"{Tr("PHRASE_RECRUIT_HERO_CONFIRMATION")} " +
                $"{PopulationDescriptionControl.Instance.populationData.firstName} " +
                $"{PopulationDescriptionControl.Instance.populationData.lastName}";
        }
    }

    /// <summary>
    /// armyLeaderRecruited is getting populated by RecruitHeroButton script.
    /// </summary>
    private void YesButton()
    {
        PopulationData populationData = PopulationDescriptionControl.Instance.populationData;
        County county = (County)Globals.Instance.countiesParent.GetChild(populationData.location);

        if (populationData.isHero == false)
        {
            Banker.ChargeForHero(Globals.Instance.playerFactionData);
        }
        // If the population isn't a hero already then it removes it from the population list.
        if (populationData.isHero != true)
        {
            county.countyData.populationDataList.Remove(populationData);
        }
        if (armyLeaderRecruited == false)
        {
            populationData.isHero = true;
            populationData.HeroType = AllEnums.HeroType.Aide;
            county.countyData.heroesInCountyList.Add(populationData);
            county.countyData.factionData.AddHeroToAllHeroesList(populationData);
        }
        else
        {
            populationData.isHero = true;
            if (populationData.HeroType == AllEnums.HeroType.FactionLeader)
            {
                populationData.HeroType = AllEnums.HeroType.FactionLeaderArmyLeader;
            }
            else
            {
                populationData.HeroType = AllEnums.HeroType.ArmyLeader;
            }

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

        MakePopulationIdle(populationData);
        PopulationDescriptionControl.Instance.UpdateDescriptionInfo();
        CountyInfoControl.Instance.GenerateHeroesPanelList();

        TopBarControl.Instance.UpdateTopBarGoodLabels();
        Hide();
    }

    private void MakePopulationIdle(PopulationData populationData)
    {
        populationData.RemoveFromCountyImprovement();
        // I don't think we need to remove them from research or scavenging.  I think.
    }

    private void NoButton()
    {
        Hide();
    }
}
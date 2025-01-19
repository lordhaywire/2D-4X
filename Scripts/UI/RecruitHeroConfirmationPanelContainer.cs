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
        CountyData countyData = Globals.Instance.GetCountyDataFromLocationID(populationData.location);

        if (populationData.isHero == false)
        {
            Banker.ChargeForHero(Globals.Instance.playerFactionData);
        }

        // If the population isn't a hero already then it removes it from the population list.
        if (populationData.isHero != true)
        {
            countyData.populationDataList.Remove(populationData);
        }
        if (armyLeaderRecruited == false)
        {
            populationData.isHero = true;
            populationData.HeroType = AllEnums.HeroType.Aide;
            countyData.heroesInCountyList.Add(populationData);
            countyData.factionData.AddHeroToAllHeroesList(populationData);
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

            countyData.heroesInCountyList.Remove(populationData);
            countyData.armiesInCountyList.Add(populationData);
            countyData.factionData.AddHeroToAllHeroesList(populationData);
        }

        // This is set again to update the sprite textures;
        // Why is there a null check here?  Does this sometimes not have a token?
        if (populationData.heroToken != null)
        {
            AllTokenTextures.Instance.AssignTokenTextures(populationData.heroToken);
            populationData.heroToken.UpdateSpriteTexture();
            populationData.heroToken.spawnedTokenButton.UpdateButtonIcon();
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
using System;
using AutoloadSpace;
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
    
    private void YesButton()
    {
        PopulationData populationData = PopulationDescriptionControl.Instance.populationData;
        populationData.ConvertPopulationToAide();

        PopulationDescriptionControl.Instance.UpdateDescriptionInfo();
        CountyInfoControl.Instance.GenerateHeroesPanelList();

        TopBarControl.Instance.UpdateTopBarGoodLabels();
        Hide();
    }

    private void NoButton()
    {
        Hide();
    }
}
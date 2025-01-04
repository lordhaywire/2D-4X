using Godot;

namespace PlayerSpace;
public partial class RemoveCountyImprovementConfirmationPanelContainer : PanelContainer
{
    public CountyImprovementData removingCountyImprovementData;
    [Export] private Panel clickBlockerPanel;
    [Export] private Label titleLabel;

    private void RemoveCountyImprovementOnVisibilityChanged()
    {
        if (Visible)
        {
            CallDeferred(nameof(GenerateConfirmationText));
            clickBlockerPanel.Show();
        }
        else
        {
            TopBarControl.Instance.UpdateResourceLabels();
            clickBlockerPanel.Hide();
        }
    }

    private void RemoveImprovementYesButtonPressed()
    {
        CountyData countyData = Globals.Instance.SelectedLeftClickCounty.countyData;
        Haulmaster.ReturnHalfOfConstructionCost(countyData, removingCountyImprovementData);

        // Remove building from Construction List
        countyData.underConstructionCountyImprovements.Remove(removingCountyImprovementData);
        countyData.completedCountyImprovements.Remove(removingCountyImprovementData);
        CountyImprovementsControl.Instance.CreateAllCountyImprovementButtons();
        Hide();
    }

    private void RemoveImprovementNoButtonPressed()
    {
        Hide();
    }
    private void GenerateConfirmationText()
    {
        titleLabel.Text = $"{Tr("PHRASE_REMOVE_IMPROVEMENT")} : {Tr(removingCountyImprovementData.improvementName)}";
    }

}

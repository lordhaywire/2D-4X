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
            TopBarControl.Instance.UpdateTopBarGoodLabels();
            clickBlockerPanel.Hide();
        }
    }

    private void RemoveImprovementYesButtonPressed()
    {
        CountyData countyData = Globals.Instance.SelectedLeftClickCounty.countyData;
        Haulmaster.ReturnHalfOfConstructionCost(countyData, removingCountyImprovementData);

        // Remove building from Construction Lists
        countyData.underConstructionCountyImprovementList.Remove(removingCountyImprovementData);
        countyData.completedCountyImprovementList.Remove(removingCountyImprovementData);

        // Check to see if it is a storage improvement and not under construction then remove the storage.
        if (removingCountyImprovementData.CheckIfStorageImprovement() && removingCountyImprovementData.status
            == AllEnums.CountyImprovementStatus.Producing)
        {
            Haulmaster.SubtractImprovementStorageFromCounty(countyData, removingCountyImprovementData);
        }
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

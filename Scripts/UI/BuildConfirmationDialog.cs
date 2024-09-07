using Godot;

namespace PlayerSpace
{
    public partial class BuildConfirmationDialog : ConfirmationDialog
    {
        private void OnVisibilityChanged()
        {
            if (Visible)
            {
                CallDeferred(nameof(UpdateConfirmationDialog));
            }
        }

        private void UpdateConfirmationDialog()
        {
            string confirmString = "PHRASE_BUILD_IMPROVEMENT_CONFIRM";
            DialogText = Tr(confirmString) + " "
                + Tr(Globals.Instance.selectedPossibleBuildingControl.countyImprovementData.improvementName) + "?";
        }

        private void YesButton()
        {
            //GD.Print("Yes was pressed.");
            CountyAI countyAI = new();
            Banker banker = new();
            CountyImprovementData countyImprovementData = Globals.Instance.selectedPossibleBuildingControl.countyImprovementData;
            
            countyAI.BuildImprovement(Globals.Instance.SelectedLeftClickCounty.countyData, countyImprovementData);
            banker.ChargeForBuilding(Globals.Instance.playerFactionData
                , Globals.Instance.selectedPossibleBuildingControl.countyImprovementData);
            
            // Removes the cost of the building.
            TopBarControl.Instance.UpdateResourceLabels();
            CountyImprovementsControl.Instance.CreateAllCountyImprovementButtons();
            Hide();
        }
    }
}
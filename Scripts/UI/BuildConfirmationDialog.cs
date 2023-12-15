using Godot;

namespace PlayerSpace
{
    public partial class BuildConfirmationDialog : ConfirmationDialog
    {
        private void OnVisibilityChanged()
        {
            CallDeferred("UpdateConfirmatioDialog");
        }

        private void UpdateConfirmatioDialog()
        {
            DialogText = AllText.DialogTitles.BUILDINGBUILDINGS 
                + Globals.Instance.selectedPossibleBuildingControl.countyImprovementData.improvementName;       
        }

        private void YesButton()
        {
            GD.Print("Yes was pressed.");
            // Maybe change this to its own method, or even have this code somewhere else.  We probably want to move this when we do AI
            // stuff.
            CountyImprovementData countyImprovementData = Globals.Instance.selectedPossibleBuildingControl.countyImprovementData;

            Globals.Instance.selectedPossibleBuildingControl
                .Reparent(CountyImprovementsControl.Instance.currentImprovementsScrollContainerParent);
            Globals.Instance.selectedPossibleBuildingControl.UpdatePossibleBuildingLabels();
            Banker.Instance.BuildImprovement(Globals.Instance.selectedSelectCounty.countyData, countyImprovementData);
            Banker.Instance.ChargeForBuilding(Globals.Instance.playerFactionData
                , Globals.Instance.selectedPossibleBuildingControl.countyImprovementData);
            Hide();
        }

        
    }
}
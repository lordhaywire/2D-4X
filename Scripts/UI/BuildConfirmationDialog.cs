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
            DialogText = AllText.DialogTitles.BUILDINGBUILDINGS 
                + Globals.Instance.selectedPossibleBuildingControl.countyImprovementData.improvementName;       
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
            
            TopBarControl.Instance.UpdateResourceLabels();
            CountyImprovementsControl.Instance.CreateAllCountyImprovementButtons();
            Hide();
        }
    }
}
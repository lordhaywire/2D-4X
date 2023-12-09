using Godot;

namespace PlayerSpace
{
	public partial class PossibleBuildingControl : Control
	{
		public CountyImprovementData countyImprovementData;

		[Export] public Label improvementNameLabel;
		[Export] private Label improvementDescriptionLabel;
		[Export] private Label improvementInfluenceCostLabel;
		[Export] private Label improvementAmountOfWorkLabel;
		[Export] private Label improvementMaxWorkersLabel;

		[Export] public Label underContructionLabel;

		[Export] private Button buildingButton;


		public override void _Ready()
		{
			CallDeferred("UpdatePossibleBuildingLabels");
		}

		private void UpdatePossibleBuildingLabels()
		{
			if(Banker.Instance.CheckBuildingCost(Globals.Instance.selectedCountyData, countyImprovementData) == false)
			{
				buildingButton.Disabled = true;
			}
			improvementNameLabel.Text = countyImprovementData.improvementName;
			improvementDescriptionLabel.Text = countyImprovementData.improvementDescription;
			improvementInfluenceCostLabel.Text = "Influence Cost: " + countyImprovementData.influenceCost.ToString();
			improvementAmountOfWorkLabel.Text = "Amount of work: " + countyImprovementData.maxAmountOfWork.ToString();
			improvementMaxWorkersLabel.Text = "Max Workers: " + countyImprovementData.maxWorkers.ToString();
			if (countyImprovementData.isBeingBuilt == true || countyImprovementData.isBuilt == true)
			{
				buildingButton.Disabled = true;
			}
		}

		private void BuildingButton()
		{
			GD.Print("You have pressed the building button.");
			CountyImprovementsControl.Instance.buildConfirmationDialog.Visible = true;
			Globals.Instance.selectedPossibleBuildingControl = this;
		}

		public void UpdateUnderContructionBuildingLabels()
		{
            improvementNameLabel.Text = countyImprovementData.improvementName;
            improvementDescriptionLabel.Text = countyImprovementData.improvementDescription;
			improvementInfluenceCostLabel.Hide();
            improvementAmountOfWorkLabel.Text = $"{countyImprovementData.currentAmountOfWork}/{countyImprovementData.maxAmountOfWork} Amount of Work";
            improvementMaxWorkersLabel.Text = $"{countyImprovementData.currentWorkers}/{countyImprovementData.maxWorkers} Workers";
        }
	}
}
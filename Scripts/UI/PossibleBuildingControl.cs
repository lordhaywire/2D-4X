using Godot;

namespace PlayerSpace
{
	public partial class PossibleBuildingControl : Control
	{
		public CountyImprovementData countyImprovementData;

		[Export] public Label improvementNameLabel;
		[Export] private Label improvementDescriptionLabel;
		[Export] private Label improvementInfluenceCostLabel;
		[Export] private Label improvementAmountOfConstructionLabel;
		[Export] private Label improvementMaxBuildersLabel;

		[Export] public Label underContructionLabel;

		[Export] private Button buildingButton;

		public override void _Ready()
		{
			CallDeferred("UpdatePossibleBuildingLabels");
		}

		public void UpdatePossibleBuildingLabels()
		{
			improvementNameLabel.Text = countyImprovementData.improvementName;
			improvementDescriptionLabel.Text = countyImprovementData.improvementDescription;
			improvementInfluenceCostLabel.Text = $"Influence Cost: {countyImprovementData.influenceCost}";
            if (Banker.Instance.CheckBuildingCost(Globals.Instance.selectedCountyData, countyImprovementData) == false)
            {
                buildingButton.Disabled = true; 
            }
            if (countyImprovementData.isBeingBuilt == true || countyImprovementData.isBuilt == true)
			{
				buildingButton.Disabled = true;
			}
			if (countyImprovementData.isBeingBuilt != true)
			{
				improvementAmountOfConstructionLabel.Text = $"Amount of work: {countyImprovementData.maxAmountOfConstruction}";
				improvementMaxBuildersLabel.Text = $"Max Workers: {countyImprovementData.maxBuilders}";
			}
			else
			{
				improvementInfluenceCostLabel.Hide();
				underContructionLabel.Show();
				improvementAmountOfConstructionLabel.Text = $"{countyImprovementData.currentAmountOfConstruction}/{countyImprovementData.maxAmountOfConstruction} Amount of Contruction";
				improvementMaxBuildersLabel.Text = $"{countyImprovementData.currentBuilders}/{countyImprovementData.maxBuilders} Builders";
			}
			if(countyImprovementData.isBuilt == true)
			{
				improvementMaxBuildersLabel.Text = $"{countyImprovementData.currentWorkers}/{countyImprovementData.maxWorkers} Workers";
				improvementAmountOfConstructionLabel.Hide();
			}
        }

		private void BuildingButton()
		{
			GD.Print("You have pressed the building button.");
			CountyImprovementsControl.Instance.buildConfirmationDialog.Visible = true;
			Globals.Instance.selectedPossibleBuildingControl = this;
		}
	}
}
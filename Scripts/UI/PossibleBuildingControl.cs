using Godot;

namespace PlayerSpace
{
	public partial class PossibleBuildingControl : Control
	{
		public CountyImprovementData countyImprovement;

		[Export] private Label improvementName;
		[Export] private Label improvementDescription;
		[Export] private Label improvementInfluenceCost;
		[Export] private Label improvementAmountOfWork;
		[Export] private Label improvementMaxWorkers;

		public override void _Ready()
		{
			CallDeferred("UpdateLabels");
		}

		private void UpdateLabels()
		{
            improvementName.Text = countyImprovement.improvementName;
            improvementDescription.Text = countyImprovement.improvementDescription;
            improvementInfluenceCost.Text = "Influence Cost: " + countyImprovement.influenceCost.ToString();
            improvementAmountOfWork.Text = "Amount of work: " + countyImprovement.amountOfWorkCost.ToString();
            improvementMaxWorkers.Text = "Max Workers: " + countyImprovement.maxWorkers.ToString();
        }
	}
}
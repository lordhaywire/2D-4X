using Godot;
using System.Linq;

namespace PlayerSpace
{
    public partial class CountyImprovementsControl : Control
    {
        public static CountyImprovementsControl Instance { get; private set; }

        [Export] private Node possibleImprovementsScrollContainerParent;
        [Export] public Node currentImprovementsScrollContainerParent;
        [Export] public ConfirmationDialog buildConfirmationDialog;

        [Export] private PackedScene countyImprovementButtonPackedScene;

        public override void _Ready()
        {
            Instance = this;
        }
        private void OnVisibilityChanged()
        {
            if (Visible == true)
            {
                Clock.Instance.PauseTime();
                GenerateCountyImprovementButtons();
                PlayerControls.Instance.AdjustPlayerControls(false);
                CountyInfoControl.Instance.populationDescriptionMarginContainer.Hide();
                CountyInfoControl.Instance.populationListMarginContainer.Hide();
            }
            else
            {
                Clock.Instance.UnpauseTime();
                PlayerControls.Instance.AdjustPlayerControls(true);
            }
        }

        private void GenerateCountyImprovementButtons()
        {
            ClearImprovements();

            foreach(CountyImprovementData countyImprovementData in Globals.Instance.selectedCountyData.allCountyImprovements)
            {
                if(countyImprovementData.isBeingBuilt == false && countyImprovementData.isBuilt == false)
                {
                    PossibleBuildingControl countyImprovementButton = (PossibleBuildingControl)countyImprovementButtonPackedScene.Instantiate();
                    possibleImprovementsScrollContainerParent.AddChild(countyImprovementButton);
                    countyImprovementButton.countyImprovementData = countyImprovementData;
                }
                else
                {
                    PossibleBuildingControl countyImprovementButton = (PossibleBuildingControl)countyImprovementButtonPackedScene.Instantiate();
                    currentImprovementsScrollContainerParent.AddChild(countyImprovementButton);
                    countyImprovementButton.countyImprovementData = countyImprovementData;
                }
            }
        }

        private void ClearImprovements()
        {
            foreach(Node node in possibleImprovementsScrollContainerParent.GetChildren().Skip(1))
            {
                node.QueueFree();
            }
            foreach (Node node in currentImprovementsScrollContainerParent.GetChildren().Skip(1))
            {
                node.QueueFree();
            }
        }
        private void CloseButton()
        {
            Hide();
        }
    }
}
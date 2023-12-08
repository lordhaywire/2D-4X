using Godot;
using System.Linq;

namespace PlayerSpace
{
    public partial class CountyImprovementsControl : Control
    {
        [Export] private Node possibleImprovementsScrollContainerParent;
        [Export] private Node currentImprovementsScrollContainerParent;

        [Export] private PackedScene countyImprovementButtonPackedScene;
        private void OnVisibilityChanged()
        {
            if (Visible == true)
            {
                Clock.Instance.PauseTime();
                GenerateCountyImprovementButtons();
            }
            else
            {
                Clock.Instance.UnpauseTime();
            }
        }

        private void GenerateCountyImprovementButtons()
        {
            ClearImprovements();

            foreach(CountyImprovementData countyImprovementData in Globals.Instance.selectedCountyData.countyImprovements)
            {
                if(countyImprovementData.isBeingBuilt == false && countyImprovementData.isBuilt == false)
                {
                    PossibleBuildingControl countyImprovementButton = (PossibleBuildingControl)countyImprovementButtonPackedScene.Instantiate();
                    possibleImprovementsScrollContainerParent.AddChild(countyImprovementButton);
                    countyImprovementButton.countyImprovement = countyImprovementData;
                }
                else
                {
                    PossibleBuildingControl countyImprovementButton = (PossibleBuildingControl)countyImprovementButtonPackedScene.Instantiate();
                    currentImprovementsScrollContainerParent.AddChild(countyImprovementButton);
                    countyImprovementButton.countyImprovement = countyImprovementData;
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
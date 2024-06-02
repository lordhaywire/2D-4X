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
                CountyInfoControl.Instance.populationDescriptionControl.Hide();
                CountyInfoControl.Instance.populationListMarginContainer.Hide();
            }
            else
            {
                Clock.Instance.UnpauseTime();
                PlayerControls.Instance.AdjustPlayerControls(true);
            }
        }

        public void GenerateCountyImprovementButtons()
        {
            ClearImprovements();

            foreach(CountyImprovementData countyImprovementData in Globals.Instance.SelectedLeftClickCounty.countyData.allCountyImprovements)
            {
                if(countyImprovementData.underConstruction == false && countyImprovementData.isBuilt == false)
                {
                    CountryImprovementDescriptionButton countyImprovementButton = (CountryImprovementDescriptionButton)countyImprovementButtonPackedScene.Instantiate();
                    possibleImprovementsScrollContainerParent.AddChild(countyImprovementButton);
                    countyImprovementButton.countyImprovementData = countyImprovementData;
                }
                else
                {
                    CountryImprovementDescriptionButton countyImprovementButton = (CountryImprovementDescriptionButton)countyImprovementButtonPackedScene.Instantiate();
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
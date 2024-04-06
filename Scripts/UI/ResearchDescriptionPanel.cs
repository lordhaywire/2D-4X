using Godot;
using System;

namespace PlayerSpace
{
    public partial class ResearchDescriptionPanel : PanelContainer
    {
        public static ResearchDescriptionPanel Instance { get; private set; }

        [Export] private Label researchName;
        [Export] private TextureRect researchTextureRect;
        [Export] private Label researchDescription;
        [Export] private Label amountOfResearchDoneLabel;
        [Export] private Label costOfResearchLabel;
        [Export] private HBoxContainer countyImprovementsInResearchParent;
        [Export] private PackedScene countyImprovementResearchPackedScene;

        public ResearchItemData researchItemData;
        public override void _Ready()
        {
            Instance = this;
        }
        private void OnVisibilityChanged()
        {
            //GD.Print("Research Description Panel!");
            RemoveLabels();
            researchName.Text = researchItemData.researchName;
            researchTextureRect.Texture = researchItemData.researchTexture;
            researchDescription.Text = researchItemData.researchDescription;
            amountOfResearchDoneLabel.Text = researchItemData.AmountOfResearchDone.ToString();
            costOfResearchLabel.Text = researchItemData.costOfResearch.ToString();

            if (researchItemData.countyImprovementDatas.Length > 0)
            {
                foreach (CountyImprovementData countyImprovementData in researchItemData.countyImprovementDatas)
                {
                    Label buildingInResearchLabel = (Label)countyImprovementResearchPackedScene.Instantiate();
                    buildingInResearchLabel.Text = countyImprovementData.improvementName;
                    countyImprovementsInResearchParent.AddChild(buildingInResearchLabel);
                }
            }

        }

        private void RemoveLabels()
        {
            foreach(Label label in countyImprovementsInResearchParent.GetChildren())
            {
                label.QueueFree();
            }
        }
    }
}

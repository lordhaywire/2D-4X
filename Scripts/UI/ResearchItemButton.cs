using Godot;

namespace PlayerSpace;

public partial class ResearchItemButton : PanelContainer
{
    [Export] public ResearchItemData researchItemData = new();
    [Export] private Label researchNameLabel;
    [Export] private Label researchPrerequisitesTitleLabel;
    [Export] private TextureRect researchIconTextureRect;
    [Export] private VBoxContainer prerequisitesParent;
    [Export] private PackedScene researchPrerequisiteLabelPackedScene;
    [Export] private Button researchButton;
    [Export] private ProgressBar researchProgressBar;

    public override void _Ready()
    {
        CallDeferred(nameof(SubscribeToResearchEvent));
        UpdateResearchButtonInfo();
    }

    private void SubscribeToResearchEvent()
    {
        ResearchControl.Instance.ResearchVisible += CheckIfResearchIsDone;
    }

    private void UpdateResearchButtonInfo()
    {
        researchNameLabel.Text = researchItemData.researchName;
        researchIconTextureRect.Texture = researchItemData.researchTexture;
        if (researchItemData.researchPrerequisites.Count > 0)
        {
            GD.Print($"{researchItemData.researchName} is not null");
            UpdatePrerequisites();
        }
        else
        {
            researchPrerequisitesTitleLabel.Hide();
        }
    }

    private void UpdatePrerequisites()
    {
        foreach(EnumsResearch.All enumResearch in researchItemData.researchPrerequisites)
        {
            Label researchPrerequisiteLabel = (Label)researchPrerequisiteLabelPackedScene.Instantiate();
            researchPrerequisiteLabel.Text 
                = Globals.Instance.playerFactionData.researchItems[(int)enumResearch].researchName;
            prerequisitesParent.AddChild(researchPrerequisiteLabel);
        }
    }

    private void CheckIfResearchIsDone()
    {
        researchProgressBar.MaxValue = researchItemData.costOfResearch;
        researchProgressBar.Value = researchItemData.AmountOfResearchDone;
    }
    private void OnButtonPressed()
    {
        ResearchDescriptionPanel.Instance.researchItemData = researchItemData;
        ResearchDescriptionPanel.Instance.Show();
    }
}
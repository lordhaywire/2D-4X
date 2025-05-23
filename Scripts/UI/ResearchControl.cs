using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace;

public partial class ResearchControl : Control
{
    public static ResearchControl Instance { get; private set; }

    [Export] private PackedScene researchItemButton;

    [Export] public Label assignedResearchersTitleLabel;
    [Export] public VBoxContainer tierOneResearchItemParent;
    [Export] public VBoxContainer tierTwoResearchItemParent;
    [Export] public VBoxContainer tierThreeResearchItemParent;
    [Export] private PackedScene assignedResearchersButton;
    [Export] private GridContainer assignedResearchersParent;

    private ResearchItemData researchItemData;

    public event Action ResearchVisible;

    public readonly List<PopulationData> assignedResearchers = [];

    public override void _Ready()
    {
        Instance = this;
        AddPlayerResearchToUi();
    }
    private void OnVisibilityChange()
    {
        if (Visible)
        {
            PlayerControls.Instance.AdjustPlayerControls(false);
            Clock.Instance.PauseTime();
            ResearchVisible?.Invoke();

            GenerateAssignedResearchers();
            CheckForResearchers();
        }
        else
        {
            PlayerControls.Instance.AdjustPlayerControls(true);
            Clock.Instance.UnpauseTime();
        }
    }

    private void CheckForResearchers()
    {
        //GD.PrintRich($"[rainbow]Assigned Researchers Count: " + assignedResearchers.Count);
        if (assignedResearchers.Count == 0)
        {
            assignedResearchersTitleLabel.Hide();
        }
        else
        {
            assignedResearchersTitleLabel.Show();
        }
    }

    public void GenerateAssignedResearchers()
    {
        ClearResearcherHBoxContainers();
        foreach (PopulationData populationData in assignedResearchers)
        {
            //GD.Print("Generate Assigned Researchers: " + populationData.firstName);
            AssignedResearcherHboxContainer researcherButton = (AssignedResearcherHboxContainer)assignedResearchersButton.Instantiate();
            researcherButton.assignedResearcherButton.Text
                = $"{populationData.firstName} {populationData.lastName}: {Tr(populationData.currentResearchItemData.researchName)}";
            researcherButton.populationData = populationData;
            // If the county population is working at a research office, then their button is disabled, so they can't be
            // removed from the research.
            /*
            if(researcherButton.populationData.isHero == false)
            {
                researcherButton.assignedResearcherCheckbox.Disabled = true;
            }
            */
            assignedResearchersParent.AddChild(researcherButton);
        }
    }

    private void ClearResearcherHBoxContainers()
    {
        foreach (AssignedResearcherHboxContainer assignedResearcherHBoxContainer in
                 assignedResearchersParent.GetChildren().Cast<AssignedResearcherHboxContainer>())
        {
            assignedResearcherHBoxContainer.QueueFree();
        }
    }

    private void ShowResearchPanel()
    {
        //GD.Print("Show the research panel!");
        Show();
        PlayerControls.Instance.AdjustPlayerControls(false);
    }

    private void CloseButton()
    {
        if (ResearchDescriptionPanel.Instance.Visible)
        {
            ResearchDescriptionPanel.Instance.Hide();
        }
        else
        {
            Hide();
            PlayerControls.Instance.AdjustPlayerControls(true);
        }
    }

    /// <summary>
    /// This overwrites the dragged and dropped researchItemData in the UI.
    /// </summary>
    private void AddPlayerResearchToUi()
    {
        foreach(ResearchItemData currentResearchItemData in Globals.Instance.playerFactionData.researchItems)
        {
            //GD.Print($"Research Item Data Tier: " + researchItemData.tier);
            switch (currentResearchItemData.tier)
            {
                case AllEnums.ResearchTiers.One:
                    ResearchItemButton tierOneResearchItemButton = (ResearchItemButton)researchItemButton.Instantiate();
                    tierOneResearchItemButton.researchItemData = currentResearchItemData;
                    tierOneResearchItemParent.AddChild(tierOneResearchItemButton);
                    break;
                case AllEnums.ResearchTiers.Two:
                    ResearchItemButton tierTwoResearchItemButton = (ResearchItemButton)researchItemButton.Instantiate();
                    tierTwoResearchItemButton.researchItemData = currentResearchItemData;
                    tierTwoResearchItemParent.AddChild(tierTwoResearchItemButton);
                    break;
                case AllEnums.ResearchTiers.Three:
                    ResearchItemButton tierThreeResearchItemButton = (ResearchItemButton)researchItemButton.Instantiate();
                    tierThreeResearchItemButton.researchItemData = currentResearchItemData;
                    tierThreeResearchItemParent.AddChild(tierThreeResearchItemButton);
                    break;
            }
        }
    }
}
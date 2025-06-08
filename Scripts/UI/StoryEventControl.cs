using Godot;
using System.Linq;

namespace PlayerSpace;

public partial class StoryEventControl : Control
{
    public static StoryEventControl Instance { get; private set; }

    public StoryEventData currentStoryEventData;
    [Export] private Label storyEventTitle;
    [Export] private Label storyEventDescription;
    [Export] private Label storyEventLocation;
    [Export] private Label storyEventRewardLabel;

    [Export] private VBoxContainer choicesVBoxContainer;
    [Export] private PackedScene storyEventChoiceButton;
    //private readonly List<Button> choiceButtons = [];

    public override void _Ready()
    {
        Instance = this;

        Clock.Instance.HourChanged += CheckForEvent;
    }

    private void OnVisibilityChanged()
    {
        if (Visible)
        {
            Clock.Instance.PauseTime();
            PlayerControls.Instance.AdjustPlayerControls(false);
            UpdateEventInfo();
        }
        else
        {
            DeleteChoiceButtons();
            PlayerControls.Instance.AdjustPlayerControls(true);
            Clock.Instance.UnpauseTime();
            CountyInfoControl.Instance.UpdateEverything();
        }
    }

    // This was just for testing.
    private void CheckForEvent()
    {
        if (Clock.Instance.Days == 0 && Clock.Instance.Hours == 3 && Globals.Instance.turnOffStoryEvents == false)
        {
            Show();
        }
    }

    private void DeleteChoiceButtons()
    {
        foreach (Button button in choicesVBoxContainer.GetChildren().Skip(4).Cast<Button>())
        {
            button.QueueFree();
        }
    }


    private void UpdateEventInfo()
    {
        storyEventTitle.Text = currentStoryEventData.storyEventTitle;
        storyEventDescription.Text = currentStoryEventData.storyEventDescription;
        storyEventRewardLabel.Text =
            $"{Tr(currentStoryEventData.rewardGood.goodName)}: {currentStoryEventData.rewardAmount}";
        storyEventLocation.Text = $"{Tr("WORD_COUNTY")}: {currentStoryEventData.eventCounty.countyData.countyName}";

        InstantiateChoicesButtons();
    }

    private void InstantiateChoicesButtons()
    {
        for (int i = 0; i < currentStoryEventData.choices.Length; i++)
        {
            Button choiceButton = (Button)storyEventChoiceButton.Instantiate();
            choicesVBoxContainer.AddChild(choiceButton);
            choiceButton.Text = currentStoryEventData.choices[i];
            choiceButton.Pressed += HideStoryEventControl;

            if (currentStoryEventData.choices.Length > 1 && i == 0)
            {
                choiceButton.Pressed += AcceptButtonPressed;
            }
        }
    }

    private void HideStoryEventControl()
    {
        Hide();
    }

    private void AcceptButtonPressed()
    {
        Banker.AddStoryEventCountyGood(currentStoryEventData);
        GD.Print("Accept Button has been pressed.");
    }
}
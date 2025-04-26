using Godot;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace;

public partial class StoryEventControl : Control
{
    private StoryEventData currentStoryEventData;
    [Export] private Label storyEventTitle;
    [Export] private Label storyEventDescription;
    [Export] private Label storyEventLocation;

    private readonly List<Button> choiceButtons = [];

    public override void _Ready()
    {
        Clock.Instance.HourChanged += CheckForEvent;
        GetChoiceButtons();
    }

    private void GetChoiceButtons()
    {
        foreach (Button button in GetChild(0).GetChild(0).GetChildren().Skip(3).Cast<Button>())
        {
            choiceButtons.Add(button);
        }
    }

    private void CheckForEvent()
    {
        if (Clock.Instance.Days == 0 && Clock.Instance.Hours == 3 && Globals.Instance.turnOffStoryEvents == false)
        {
            Show();
        }
    }

    private void OnVisibilityChanged()
    {
        if (Visible)
        {
            Clock.Instance.PauseAndUnpause();
            PlayerControls.Instance.AdjustPlayerControls(false);
            UpdateEventInfo();
        }
        else
        {
            PlayerControls.Instance.AdjustPlayerControls(true);
            Clock.Instance.PauseAndUnpause();
        }
    }

    private void UpdateEventInfo()
    {
        // Hide the middle 2 buttons just in case they were shown before.
        HideButtons();

        // This is some hard coded bullshit just for the test fish event.
        currentStoryEventData = StoryEventList.Instance.storyEventDatas[0];
        currentStoryEventData.eventCounty = (County)Globals.Instance.countiesParent.GetChild(3);

        storyEventTitle.Text = currentStoryEventData.storyEventTitle;
        storyEventDescription.Text = currentStoryEventData.storyEventDescription;
        storyEventLocation.Text = $"County: {currentStoryEventData.eventCounty.countyData.countyName}";

        // The top button and the bottom button are accepted and decline, and the first string and second string
        // choices are accepted and decline;
        switch (currentStoryEventData.choices.Length)
        {
            case 3:
                choiceButtons[1].Show();
                choiceButtons[1].Text = currentStoryEventData.choices[2];
                break;
            case 4:
                choiceButtons[1].Show();
                choiceButtons[1].Text = currentStoryEventData.choices[2];
                choiceButtons[2].Show();
                choiceButtons[2].Text = currentStoryEventData.choices[3];
                break;
        }
        choiceButtons[0].Text = currentStoryEventData.choices[0];
        choiceButtons[3].Text = currentStoryEventData.choices[1];

    }

    private void HideButtons()
    {
        choiceButtons[1].Hide();
        choiceButtons[2].Hide();
    }
    private void AcceptButtonPressed()
    {
        Banker.AddStoryEventCountyGood(currentStoryEventData);
        Hide();
        GD.Print("Accept Button has been pressed.");
    }

    private void DeclineButtonPressed()
    {
        Hide();
    }
}
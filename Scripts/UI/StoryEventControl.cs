using System;
using Godot;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;

namespace PlayerSpace;

public partial class StoryEventControl : Control
{
    public static StoryEventControl Instance { get; private set; }

    public StoryEventData currentStoryEventData;
    [Export] private Label storyEventTitle;
    [Export] private Label storyEventDescription;
    [Export] private Label storyEventLocation;

    private readonly List<Button> choiceButtons = [];

    public override void _Ready()
    {
        Instance = this;

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

    // This was just for testing.
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
            Clock.Instance.PauseTime();
            
            PlayerControls.Instance.AdjustPlayerControls(false);
            UpdateEventInfo();
        }
        else
        {
            DisconnectAllSignals();
            PlayerControls.Instance.AdjustPlayerControls(true);
            Clock.Instance.UnpauseTime();
        }
    }

    private void DisconnectAllSignals()
    {
        
        foreach (Button button in choiceButtons)
        {
            Array<Dictionary> signalConnections = button.GetSignalConnectionList("pressed");
            foreach (Dictionary connections in signalConnections)
            {
                GD.Print(connections);
                foreach (string key in connections.Keys)
                {
                    GD.Print($"Key: {key}, Value: {connections[key]}");
                }
            }

            foreach (Dictionary connections in signalConnections)
            {
                string signalName = (string)connections["signal"];
                Callable callable = (Callable)connections["callable"]; 
                string method = (string)connections["flags"];

                button.Disconnect(signalName, callable);
            }
        }
    }

    private void UpdateEventInfo()
    {
        storyEventTitle.Text = currentStoryEventData.storyEventTitle;
        storyEventDescription.Text = currentStoryEventData.storyEventDescription;
        storyEventLocation.Text = $"{Tr("WORD_COUNTY")}: {currentStoryEventData.eventCounty.countyData.countyName}";

        // Hide all the choice's buttons.
        HideChoicesButtons();

        // The top button and the bottom button are accepted and decline, and the first string and second string
        // choices are accepted and decline.
        for (int i = 0;
             i < currentStoryEventData.choices.Length;
             i++)
        {
            choiceButtons[i].Text = currentStoryEventData.choices[i];
            choiceButtons[i].Show();
            choiceButtons[i].Pressed += HideStoryEventControl;
            
            if (i == 0 && currentStoryEventData.choices.Length > 1)
            {
                choiceButtons[i].Pressed += AcceptButtonPressed;
            }
        }
    }
    

    /*
if (button.IsConnected("pressed", new Callable(this, "HideStoryEventControl")))
{
    GD.PrintRich($"[rainbow]HideStoryEventControl Callable Weird Thing Worked!");
    button.Pressed -= HideStoryEventControl;
}
*/
/*
    if (button.IsConnected("pressed", new Callable(this, "AcceptButtonPressed")))
    {
        GD.PrintRich($"[rainbow]AcceptButtonPressed Callable Weird Thing Worked!");
                
        //button.Pressed -= AcceptButtonPressed;
    }
    */
    private void HideChoicesButtons()
    {
        foreach (Button button in choiceButtons)
        {
            button.Hide();
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
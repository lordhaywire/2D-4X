using Godot;

namespace PlayerSpace;

public partial class EventLog: ScrollContainer
{
    public static EventLog Instance { get; private set; }

    [Export] private ScrollContainer eventLogScrollContainer;
    [Export] private VBoxContainer eventLogVBoxContainer; // Holds all the panels.
    [Export] private PackedScene eventLogPanel; // For odd lines

    [Export] private int maxLines = 20;
    private bool isNextLogOdd = true; // Start with odd line

    public override void _Ready()
    {
        Instance = this;
    }

    public void AddLog(string newLog)
    {
        //GD.Print($"Event Log String: {newLog}");
        EventLogTextPanel textPanel;

        textPanel = (EventLogTextPanel)eventLogPanel.Instantiate();

        // Add the new log with the appropriate color prefab based on the odd/even flag
        if (isNextLogOdd == true)
        {
            textPanel.logText.AddThemeColorOverride("font_color", Colors.DodgerBlue);
        }
        else
        {
            textPanel.logText.AddThemeColorOverride("font_color", Colors.Tomato);
        }

        eventLogVBoxContainer.AddChild(textPanel);

        textPanel.logText.Text = newLog;

        if (eventLogVBoxContainer.GetChildren().Count > maxLines)
        {
            // Destroy the corresponding Node in the UI
            // This has to be Free, not QueueFree because multiple events are sometimes created in a frame.
            eventLogVBoxContainer.GetChild(0).Free();
        }

        // Toggle the odd/even flag for the next log entry
        isNextLogOdd = !isNextLogOdd;

        VScrollBar vScrollBar = eventLogScrollContainer.GetVScrollBar();
        vScrollBar.Value = vScrollBar.MaxValue;
    }
}
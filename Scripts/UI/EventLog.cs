using Godot;

namespace PlayerSpace
{
    public partial class EventLog: Control
    {
        public static EventLog Instance { get; private set; }

        [Export] private VBoxContainer eventLogVBoxContainer; // Holds all the panels.
        [Export] private PackedScene eventLogPanel; // For odd lines

        [Export] private int maxLines = 13;
        private bool isNextLogOdd = true; // Start with odd line

        public override void _Ready()
        {
            Instance = this;
        }

        public void AddLog(string newLog)
        {
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
            GD.Print(newLog);

            if (eventLogVBoxContainer.GetChildCount() > maxLines)
            {
                // Destroy the corresponding Node in the UI
                eventLogVBoxContainer.GetChild(0).QueueFree();
            }

            // Toggle the odd/even flag for the next log entry
            isNextLogOdd = !isNextLogOdd;
        }
    }

}
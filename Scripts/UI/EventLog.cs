using Godot;

namespace PlayerSpace
{
    public partial class EventLog: ScrollContainer
    {
        public static EventLog Instance { get; private set; }

        [Export] private ScrollContainer eventLogScrollContainer;
        [Export] private VBoxContainer eventLogVBoxContainer; // Holds all the panels.
        [Export] private PackedScene eventLogPanel; // For odd lines

        [Export] private int maxLines = 13;
        private bool isNextLogOdd = true; // Start with odd line

        public override void _Ready()
        {
            Instance = this;
            //eventLogScrollContainer.SetDeferred(nameof(eventLogScrollContainer.ScrollVertical), 9999);
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

            if (eventLogVBoxContainer.GetChildren().Count > maxLines)
            {
                // Destroy the corresponding Node in the UI
                eventLogVBoxContainer.GetChild(0).QueueFree();
            }

            // Toggle the odd/even flag for the next log entry
            isNextLogOdd = !isNextLogOdd;
            
            // None of this seems to fix the scroll bar being at the bottom, like it should.
            //eventLogScrollContainer.SetDeferred("scrollVertical", 9999);

            VScrollBar vScrollBar = eventLogScrollContainer.GetVScrollBar();
            //vScrollBar.Name = "FuckingScrollBar.";
            //vScrollBar.MaxValue = 9999;
            GD.Print("Scroll Bar Max Value: " + vScrollBar.MaxValue);

            vScrollBar.Value = vScrollBar.MaxValue;
            GD.Print("Scroll Bar Value: " + vScrollBar.Value);
        }
    }
}
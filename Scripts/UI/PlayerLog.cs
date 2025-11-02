using Godot;

namespace PlayerSpace;

public partial class PlayerLog: ScrollContainer
{
    public static PlayerLog Instance { get; private set; }

    [Export] private VBoxContainer playerLogVBoxContainer; // Holds all the panels.
    [Export] private PackedScene playerLogPanel; // For odd lines

    [Export] private int maxLines = 20;
    private bool isNextLogOdd = true; // Start with odd line

    public override void _Ready()
    {
        Instance = this;
        SubscribeToEvents();
    }

    /// <summary>
    /// The way we could do this is that it only subscribes to events that the player is a part of, or when we put in the player log selectable events,
    /// it would only subscribe to those events.
    /// </summary>
    private void SubscribeToEvents()
    {
        EventBus.Subscribe<BattleLostPlayerLogEvent>(HandleBattleLost);
        EventBus.Subscribe<ArmyLostPlayerLogEvent>(HandleArmyLost);
        EventBus.Subscribe<CountyCapturedEvent>(HandleCountyCaptured);
    }

    private void HandleCountyCaptured(CountyCapturedEvent capturedCountyEvent)
    {
        CountyData countyData = Globals.Instance.GetCountyDataFromLocationId(capturedCountyEvent.capturedCountyId);
        FactionData factionData = FactionData.GetFactionDataFromId(countyData.factionId);
        AddLog($"{factionData.factionName} : {countyData.countyName} : {Tr("WORD_CAPTURED")}.");
    }
    private void HandleArmyLost(ArmyLostPlayerLogEvent armyLostPlayerLogEvent)
    {
        AddLog($"{armyLostPlayerLogEvent.factionName} : {armyLostPlayerLogEvent.countyName} : {Tr("PHRASE_HAS_LOST_AN_ARMY")}.");
    }

    private void HandleBattleLost(BattleLostPlayerLogEvent battleLostPlayerLogEvent)
    {
        AddLog($"{battleLostPlayerLogEvent.factionName} : {battleLostPlayerLogEvent.battle.battleLocation.countyName} : " +
            $"{Tr("PHRASE_LOST_BATTLE")}.");
    }
    
    public void AddLog(string newLog)
    {
        //GD.Print($"Event Log String: {newLog}");
        EventLogTextPanel textPanel;

        textPanel = (EventLogTextPanel)playerLogPanel.Instantiate();

        // Add the new log with the appropriate color prefab based on the odd/even flag
        if (isNextLogOdd == true)
        {
            textPanel.logText.AddThemeColorOverride("font_color", Colors.DodgerBlue);
        }
        else
        {
            textPanel.logText.AddThemeColorOverride("font_color", Colors.Tomato);
        }

        playerLogVBoxContainer.AddChild(textPanel);

        textPanel.logText.Text = newLog;

        if (playerLogVBoxContainer.GetChildren().Count > maxLines)
        {
            // Destroy the corresponding Node in the UI
            // This has to be Free, not QueueFree because multiple events are sometimes created in a frame.
            playerLogVBoxContainer.GetChild(0).Free();
        }

        // Toggle the odd/even flag for the next log entry
        isNextLogOdd = !isNextLogOdd;

        VScrollBar vScrollBar = GetVScrollBar();
        vScrollBar.Value = vScrollBar.MaxValue;
    }
}
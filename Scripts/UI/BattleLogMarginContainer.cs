using System.Linq;
using Godot;

namespace PlayerSpace;

public partial class BattleLogMarginContainer : MarginContainer
{
    public static BattleLogMarginContainer Instance { get; private set; }

    [Export] private Texture2D armyLeaderTextureRect;
    [Export] private Texture2D heroTextureRect;
    [Export] private Texture2D soldierTextureRect;
    [Export] private PackedScene combatantScene;
    [Export] private PackedScene combatLogScene;
    [Export] private Label battleLogControlTitle;
    [Export] private VBoxContainer attackerVboxContainer;
    [Export] private VBoxContainer defenderVboxContainer;
    [Export] private VBoxContainer logVboxContainer;
    [Export] private Button closeButton;

    private bool isNextLogOdd = true; // Start with odd line
    private int maxLines = 20;

    public Battle battle;

    public override void _Ready()
    {
        Instance = this;
        ConnectSignals();
    }

    private void ConnectSignals()
    {
        closeButton.Pressed += CloseBattleLog;
        VisibilityChanged += OnBattleLogControlVisibilityChanged;
    }

    private void CloseBattleLog()
    {
        Hide();
    }

    private void OnBattleLogControlVisibilityChanged()
    {
        if (Visible)
        {
            CameraControls.Instance.cameraControlsEnabled = false;
            GenerateCombatants();
        }
        else
        {
            CameraControls.Instance.cameraControlsEnabled = true;
        }
    }

    private void GenerateCombatants()
    {
        ClearCombatants(attackerVboxContainer);
        ClearCombatants(defenderVboxContainer);

        GD.Print("Attacking Army Count: " + battle.attackingArmy.Count);
        foreach (PopulationData populationData in battle.attackingArmy)
        {
            CreateCombatant(attackerVboxContainer, populationData);
        }

        foreach (PopulationData populationData in battle.defendingArmy)
        {
            CreateCombatant(defenderVboxContainer, populationData);
        }
    }

    private void CreateCombatant(VBoxContainer vBoxContainer, PopulationData populationData)
    {
        CombatantHBoxContainer combatantHBoxContainer = (CombatantHBoxContainer)combatantScene.Instantiate();
        vBoxContainer.AddChild(combatantHBoxContainer);

        combatantHBoxContainer.nameLabel.Text = populationData.GetFullName();
        combatantHBoxContainer.statusLabel.Text = Tr(populationData.combatStatus.ToString());

        if (populationData == battle.attackingArmy[0] || populationData == battle.defendingArmy[0])
        {
            combatantHBoxContainer.heroTypeTextureRect.Texture = armyLeaderTextureRect;
            return;
        }

        if (populationData.isHero)
        {
            combatantHBoxContainer.heroTypeTextureRect.Texture = heroTextureRect;
            return;
        }
        combatantHBoxContainer.heroTypeTextureRect.Texture = soldierTextureRect;
        
    }

    private void ClearCombatants(VBoxContainer vboxContainer)
    {
        foreach (HBoxContainer hBoxContainer in vboxContainer.GetChildren().Skip(2).Cast<HBoxContainer>())
        {
            hBoxContainer.QueueFree();
        }
    }

    public void AddLog(string newLog)
    {
        CombatLogTextPanel textPanel = (CombatLogTextPanel)combatLogScene.Instantiate();

        if (isNextLogOdd)
        {
            logVboxContainer.AddChild(textPanel);
            logVboxContainer.MoveChild(textPanel, 0);
            textPanel.logText.AddThemeColorOverride("font_color", Colors.Tomato);
        }
        else
        {
            logVboxContainer.AddChild(textPanel);
            logVboxContainer.MoveChild(textPanel, 0);
            textPanel.logText.AddThemeColorOverride("font_color", Colors.DodgerBlue);
        }

        isNextLogOdd = !isNextLogOdd;
        textPanel.logText.Text = $"{Tr("WORD_DAY")}: {Clock.Instance.GetDateAndTime()} \n {newLog}";
        //GD.Print($"Attacker: {attackerVboxContainer.GetChildCount()} vs {maxLines}");
        if (logVboxContainer.GetChildCount() > maxLines)
        {
            // Destroy the corresponding Node in the UI
            int lastAttackerChild = logVboxContainer.GetChildCount() - 1;
            logVboxContainer.GetChild(lastAttackerChild).Free();
        }

        //GD.Print($"Defender: {defenderVboxContainer.GetChildCount()} vs {maxLines}");
        if (logVboxContainer.GetChildCount() > maxLines)
        {
            int lastDefenderChild = logVboxContainer.GetChildCount() - 1;
            logVboxContainer.GetChild(lastDefenderChild).Free();
        }
    }
}
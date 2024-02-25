using Godot;

namespace PlayerSpace
{
	public partial class BattleLogControl : Control
	{
        public static BattleLogControl Instance { get; private set; }

        [Export] private PackedScene combatLogScene;
        [Export] private Label battleLogControlTitle;
        [Export] private VBoxContainer defenderVboxContainer;
		[Export] private VBoxContainer attackerVboxContainer;

        private bool isNextLogOdd = true; // Start with odd line
        [Export] private int maxLines;


        private void OnVisibilityChanged()
        {
            if(Visible == true)
            {
                CameraControls.Instance.cameraControlsEnabled = false;
            }
            else
            {
                CameraControls.Instance.cameraControlsEnabled = true;
            }
        }
        public override void _Ready()
        {
            Instance = this;
        }

        public void AddLog(string newLog, bool isAttacker)
        {
            CombatLogTextPanel textPanel;
            textPanel = (CombatLogTextPanel)combatLogScene.Instantiate();

            if (isAttacker == true)
            {
                attackerVboxContainer.AddChild(textPanel);
                attackerVboxContainer.MoveChild(textPanel, 0);
                textPanel.logText.AddThemeColorOverride("font_color", Colors.Tomato);
            }
            else
            {
                defenderVboxContainer.AddChild(textPanel);
                defenderVboxContainer.MoveChild(textPanel, 0);
                textPanel.logText.AddThemeColorOverride("font_color", Colors.DodgerBlue);
            }

            textPanel.logText.Text = $"Day: {Clock.Instance.days} " 
                + string.Format("{0:00}:{1:00}", Clock.Instance.Hours, Clock.Instance.minutes) + " - " + newLog;
            //GD.Print($"Attacker: {attackerVboxContainer.GetChildCount()} vs {maxLines}");
            if (attackerVboxContainer.GetChildCount() > maxLines) 
            {
                // Destroy the corresponding Node in the UI
                int lastAttackerChild = attackerVboxContainer.GetChildCount() - 1;
                attackerVboxContainer.GetChild(lastAttackerChild).Free();
            }

            //GD.Print($"Defender: {defenderVboxContainer.GetChildCount()} vs {maxLines}");
            if(defenderVboxContainer.GetChildCount() > maxLines)
            {
                int lastDefenderChild = defenderVboxContainer.GetChildCount() - 1;
                defenderVboxContainer.GetChild(lastDefenderChild).Free();
            }

            UpdateBattleTitle();
        }

        private void UpdateBattleTitle()
        {
            
        }

        private void CloseButton()
        {
            Hide();
        }
    }
}
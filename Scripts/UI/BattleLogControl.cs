using Godot;

namespace PlayerSpace
{
	public partial class BattleLogControl : Control
	{
        public static BattleLogControl Instance { get; private set; }

        [Export] private PackedScene combatLogScene;
		[Export] private VBoxContainer defenderVboxContainer;
		[Export] private VBoxContainer attackerVboxContainer;
        private bool isNextLogOdd = true; // Start with odd line
        private int maxLines;


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

            // Add the new log with the appropriate color prefab based on the odd/even flag
            /*
            if (isNextLogOdd == true)
            {
            }
            else
            {
            }
            */

            if (isAttacker == true)
            {
                attackerVboxContainer.AddChild(textPanel);
                textPanel.logText.AddThemeColorOverride("font_color", Colors.Tomato);
            }
            else
            {
                defenderVboxContainer.AddChild(textPanel);
                textPanel.logText.AddThemeColorOverride("font_color", Colors.DodgerBlue);
            }


            textPanel.logText.Text = newLog;

            if (attackerVboxContainer.GetChildCount() > maxLines) 
            {
                // Destroy the corresponding Node in the UI
                attackerVboxContainer.GetChild(0).QueueFree();
            }

            if(defenderVboxContainer.GetChildCount() > maxLines)
            {
                defenderVboxContainer.GetChild(0).QueueFree();
            }

            // Toggle the odd/even flag for the next log entry
            //isNextLogOdd = !isNextLogOdd;
        }

        private void CloseButton()
        {
            Hide();
        }
    }
}
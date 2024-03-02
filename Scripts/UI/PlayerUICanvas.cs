using Godot;
using System;

namespace PlayerSpace
{
    public partial class PlayerUICanvas : CanvasLayer
    {
        public static PlayerUICanvas Instance { get; private set; }

        [Export] public Control BattleLogControl;
        [Export] public HeroPanelContainer selectedHeroPanelContainer;

        public override void _Ready()
        {
            Instance = this;
        }

        private void OnMouseEnterUI()
        {
            //GD.Print("Mouse entered a UI Element.");
            PlayerControls.Instance.stopClickThrough = true;
        }

        private void OnMouseExitUI()
        {
            //GD.Print("Mouse exited a UI Element.");
            PlayerControls.Instance.stopClickThrough = false;
        }
    }
}
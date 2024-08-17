using Godot;
using System;

namespace PlayerSpace
{
    public partial class PlayerUICanvas : CanvasLayer
    {
        public static PlayerUICanvas Instance { get; private set; }

        [Export] public Control BattleLogControl; // This probably should be BattleLog thing, not a control.
        [Export] public PopulationListUIElement populationListUIElement;
        [Export] public HeroPanelContainer selectedHeroPanelContainer;
        [Export] public PanelContainer resourcesPanelContainer;


        public override void _Ready()
        {
            Instance = this;
        }

        private static void OnMouseEnterUI()
        {
            //GD.Print("Mouse entered a UI Element.");
            PlayerControls.Instance.stopClickThrough = true;
        }

        private static void OnMouseExitUI()
        {
            //GD.Print("Mouse exited a UI Element.");
            PlayerControls.Instance.stopClickThrough = false;
        }
    }
}
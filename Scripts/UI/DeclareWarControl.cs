using Godot;
using System;

namespace PlayerSpace
{
    public partial class DeclareWarControl : Control
    {
        public static DeclareWarControl Instance { get; private set; }

        [Export] public ConfirmationDialog confirmationWarDialog;
        public override void _Ready()
        {
            Instance = this;
        }

        private void OnVisibilityChanged()
        {
            confirmationWarDialog.Visible = Visible;
        }

        private void YesButton()
        {
            Hide();
            War newWar = new();
            newWar.attackerFactionData = Globals.Instance.playerFactionData;    
            newWar.defenderFactionData = Globals.Instance.selectedRightClickCounty.countyData.factionData;
            GD.Print($"{newWar.attackerFactionData.factionName} is attacking {newWar.defenderFactionData.factionName}");
            // Once yes has been hit, this is the actual declaration of war.
            Globals.Instance.playerFactionData.diplomacy.DeclareWar(newWar);
            SelectToken selectToken = Globals.Instance.SelectedCountyPopulation.token;
            selectToken.tokenMovement.StartMove(Globals.Instance.selectedRightClickCounty.countyData.countyId);
        }

        private void CancelButton()
        {
            
            Hide();
        }
    }
}
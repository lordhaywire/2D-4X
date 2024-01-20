using Godot;
using System;

namespace PlayerSpace
{
    public partial class SelectCounty : Node2D
    {
        [Export] public CountyData countyData;

        [ExportGroup("Attached Nodes")]
        [Export] public Sprite2D maskSprite;
        [Export] public Sprite2D countySprite;
        [Export] public Sprite2D capitalSprite;
        [Export] public Node2D countyOverlayNode2D;
        [Export] public HeroStacker heroSpawn;
        [Export] public BattleControl battleControl;

        private SelectToken selectToken; 
        private CountyPopulation countyPopulation;

        private Color outlineColor = new(0, 0, 0, 0.7f);
        private Color fillColor = new(1, 1, 1, 0.345f);
        public override void _Ready()
        {
            countyData.CountySelected += SelectedChanged;
        }

        

        public void DeclareWarConfirmation()
        {
            DeclareWarControl.Instance.Show();
            DeclareWarControl.Instance.confirmationWarDialog.DialogText 
                = AllText.Diplomacies.DECLAREWARE + countyData.factionData.factionName; 
        }

        public void StartMove()
        {
            countyPopulation = Globals.Instance.CurrentlySelectedToken.countyPopulation;
            //GD.Print("County Data: " + countyData.countyID);

            GD.Print($"{Globals.Instance.CurrentlySelectedToken.countyPopulation.firstName} has location of {countyPopulation.location}");
            SelectCounty selectLocationCounty
                = (SelectCounty)Globals.Instance.countiesParent.GetChild(countyPopulation.location);
            Globals.Instance.heroMoveTarget = heroSpawn.GlobalPosition;

            countyPopulation.destination = countyData.countyId;
            countyPopulation.currentActivity = AllText.Activities.MOVING;
            Globals.Instance.CurrentlySelectedToken.tokenMovement.MoveToken = true;

            // Remove countyPopulation from the heroes starting county location list.
            selectLocationCounty.countyData.heroCountyPopulation.Remove(countyPopulation);

            // Removed from spawnedTokenList in starting county location.
            selectLocationCounty.heroSpawn.spawnedTokenList.Remove(selectToken);
        }

        // This controls the masks in the county.  Needs to be updated.
        private void SelectedChanged(bool selected)
        {
            if (selected)
            {
                ((ShaderMaterial)maskSprite.Material).SetShaderParameter("outline_color", outlineColor);
                ((ShaderMaterial)maskSprite.Material).SetShaderParameter("fill_color", fillColor);
            }
            else
            {
                ((ShaderMaterial)maskSprite.Material).SetShaderParameter("outline_color", outlineColor.A * 0.5f);
                ((ShaderMaterial)maskSprite.Material).SetShaderParameter("fill_color", fillColor.A * 0.5f);
            }
        }
    }
}


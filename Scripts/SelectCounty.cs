using Godot;
using System;
using static PlayerSpace.HeroStacker;

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
        //[Export] public HeroStacker heroSpawn;
        [Export] public HeroTokensControl heroTokensControl;

        [Export] public HBoxContainer armiesHBox;
        [Export] public HBoxContainer heroesHBox;

        private SelectToken selectToken; 
        private CountyPopulation countyPopulation;

        //public ListWithNotify<SelectToken> spawnedHeroList = new(); // This is not a normal list.
        //public ListWithNotify<SelectToken> spawnedArmyList = new(); // This is not a normal list.


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
            Globals.Instance.heroMoveTarget = capitalSprite.GlobalPosition;

            countyPopulation.destination = countyData.countyId;
            countyPopulation.currentActivity = AllText.Activities.MOVING;
            Globals.Instance.CurrentlySelectedToken.tokenMovement.MoveToken = true;

            // Remove countyPopulation from the heroes starting county location list.
            selectLocationCounty.countyData.heroCountyPopulation.Remove(countyPopulation);

            countyPopulation.token.Show();

            // Removed from spawnedTokenList in starting county location.
            //selectLocationCounty.capitalSprite.spawnedTokenList.Remove(selectToken);
        }
    }
}


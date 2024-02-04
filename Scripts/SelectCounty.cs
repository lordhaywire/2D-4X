using Godot;
using System.Collections.Generic;


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
        [Export] public Node2D heroSpawn;
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

        public void StartMove() // Let's move this to token movement.
        {
            
            countyPopulation = Globals.Instance.selectedCountyPopulation;
            SelectToken selectToken = (SelectToken)countyPopulation.token;

            //GD.Print("County Data: " + countyData.countyID);

            GD.Print($"{countyPopulation.firstName} has location of {countyPopulation.location}");
            SelectCounty selectLocationCounty
                = (SelectCounty)Globals.Instance.countiesParent.GetChild(countyPopulation.location);
            Globals.Instance.heroMoveTarget = heroSpawn.GlobalPosition;

            countyPopulation.destination = countyData.countyId;
            countyPopulation.currentActivity = AllText.Activities.MOVING;
            selectToken.tokenMovement.MoveToken = true;

            countyPopulation.token.Show();
        }
    }
}


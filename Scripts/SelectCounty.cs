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

            SelectToken selectToken = (SelectToken)Globals.Instance.selectedCountyPopulation.token;
            selectToken.tokenMovement.StartMove(countyData.countyId);

        }
    }
}


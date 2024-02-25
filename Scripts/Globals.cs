using Godot;
using System.Collections.Generic;

namespace PlayerSpace
{
    public partial class Globals : Node
    {
        public static Globals Instance { get; private set; }

        [ExportGroup("Event Variables")]
        public List<FactionData> factionDatas = [];

        [Export] public int dailyInfluenceGain;

        [ExportGroup("Game Settings")]
        [Export] public bool startPaused;

        [ExportGroup("Selected Items")]
        [Export] public int selectedCountyId;
        [Export] private CountyData selectedCountyData;

        public CountyData SelectedCountyData
        {
            get { return selectedCountyData; }
            set
            {
                if (selectedCountyData != null)
                {
                    SelectCounty county = (SelectCounty)selectedCountyData.countyNode;
                    county.maskSprite.SelfModulate = new Color(1, 1, 1, 1f);
                }
                selectedCountyData = value;
                SelectCounty newCounty = (SelectCounty)selectedCountyData.countyNode;
                newCounty.maskSprite.SelfModulate = new Color(0, 0, 0, 1f);
            }
        }
        [Export] public SelectCounty selectedLeftClickCounty;
        [Export] public SelectCounty selectedRightClickCounty;
        private CountyPopulation selectedCountyPopulation;
        public CountyPopulation SelectedCountyPopulation
        {
            get { return selectedCountyPopulation; }
            set
            {
                selectedCountyPopulation = value;
                if(selectedCountyPopulation == null)
                {
                    PlayerUICanvas.Instance.selectedHeroPanelContainer.Hide();
                }
                else
                {
                    CallDeferred("UpdateSelectedHero");
                    PlayerUICanvas.Instance.selectedHeroPanelContainer.Show();
                }
            }
        }
        public PossibleBuildingControl selectedPossibleBuildingControl;
        public bool isVisitorList;

        [ExportGroup("Map")]
        [Export] public string pathToCounties = "res://Counties/";
        [Export] public Texture2D mapColorCoded;


        [ExportGroup("Population Generation")]
        [Export] public Node2D countiesParent; // Used for Population generation and random color.  I think we are going to change how the colors are distubuted.
        //[Export] public int heroPopulation = 1;
        [Export] public int totalCapitolPop = 10;
        [Export] public int minimumCountyPop = 1;
        [Export] public int maximumCountyPop = 4;

        [ExportGroup("Faction Shit")]
        [Export] public int minimumFood;

        [ExportGroup("Player Faction BS")]
        [Export] public FactionData playerFactionData;

        [ExportGroup("Hero Shit")]
        [Export] public PackedScene heroToken;
        [Export] public PackedScene spawnedTokenButton;
        [Export] public int movementSpeed = 10;
        [Export] public float heroStackingOffset = 3;
        [Export] public Vector2 heroMoveTarget;
        [Export] public int costOfHero;
        [Export] public int loyaltyCheckNumber = 50;
        [Export] public int moraleDamageMin = 1;
        [Export] public int moraleDamageMax = 21; // One above max.
        public int researchClicked; // This is so the Research description panel knows which research was clicked.

        string listsPath = "res://Lists/";
        string maleNamesPath = "MaleNames.txt";
        string femaleNamesPath = "FemaleNames.txt";
        string lastNamesPath = "LastNames.txt";

        public List<string> maleNames = [];
        public List<string> femaleNames = [];
        public List<string> lastNames = [];

        [ExportGroup("This is some bullshit.")]
        [Export] public bool isInsideToken;

        public override void _Ready()
        {
            Instance = this;
            LoadNames();
        }

        public void UpdateSelectedHero()
        {
            PlayerUICanvas.Instance.selectedHeroPanelContainer.countyPopulation = selectedCountyPopulation;
            CountyInfoControl.Instance.UpdateHeroInfo(PlayerUICanvas.Instance.selectedHeroPanelContainer, selectedCountyPopulation);
        }

        private void LoadNames()
        {
            // Load all the names from disk.
            GD.Print("Localize Path: " + ProjectSettings.LocalizePath(listsPath));
            GD.Print("Globalize Path: " + ProjectSettings.GlobalizePath(listsPath));
            DirAccess directory = DirAccess.Open(listsPath);
            if (directory.DirExists(listsPath))
            {
                using var maleFile = FileAccess.Open(listsPath + maleNamesPath, FileAccess.ModeFlags.Read);
                while (maleFile.GetPosition() < maleFile.GetLength())
                {
                    maleNames.Add(maleFile.GetLine());
                }
                using var femaleFile = FileAccess.Open(listsPath + femaleNamesPath, FileAccess.ModeFlags.Read);
                while (femaleFile.GetPosition() < femaleFile.GetLength())
                {
                    femaleNames.Add(femaleFile.GetLine());
                }
                using var lastNameFile = FileAccess.Open(listsPath + lastNamesPath, FileAccess.ModeFlags.Read);
                while (lastNameFile.GetPosition() < lastNameFile.GetLength())
                {
                    lastNames.Add(lastNameFile.GetLine());
                }
            }
            else
            {
                GD.Print("Directory doesn't exist!");
            }
        }
    }
}


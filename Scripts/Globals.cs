using Godot;
using System.Collections.Generic;

namespace PlayerSpace
{
    public partial class Globals : Node2D
    {
        public static Globals Instance { get; private set; }

        [ExportGroup("Selected Items")]
        [Export] public CountyData selectedCountyData;
        [Export] public Node2D selectedCounty;
        [Export] public CountyPopulation selectedCountyPopulation;
        [Export] private SelectToken currentlySelectedToken;

        [ExportGroup("Token Textures")]
        [Export] private Texture2D selectedHeroTexture;
        [Export] private Texture2D unselectedHeroTexture;

        public SelectToken CurrentlySelectedToken
        {
            get
            {
                return currentlySelectedToken;
            }
            set
            {
                if (currentlySelectedToken != null)
                {
                    currentlySelectedToken.sprite.Texture = unselectedHeroTexture;
                }
                currentlySelectedToken = value;
                if (currentlySelectedToken != null)
                {
                    currentlySelectedToken.sprite.Texture = selectedHeroTexture;                  
                }
            }
        }

        [ExportGroup("Population Generation")]
        [Export] public Node2D countiesParent; // Used for Population generation and random color.  I think we are going to change how the colors are distubuted.
        //[Export] public int heroPopulation = 1;
        [Export] public int totalCapitolPop = 10;
        [Export] public int minimumCountyPop = 1;
        [Export] public int maximumCountyPop = 4;

        [ExportGroup("Controls")]
        [Export] public bool playerControlsEnabled = true;

        [ExportGroup("County Info")]
        [Export] public Control countyInfoControl;
        [Export] public Label countyNameLabel;

        [ExportGroup("Hero Shit")]
        [Export] public PackedScene heroToken;
        [Export] public int movementSpeed = 10;
        [Export] public float heroStackingOffset = 3;
        [Export] public Vector2 heroMoveTarget;
        [Export] public int costOfHero;

        public int researchClicked; // This is so the Research description panel knows which research was clicked.

        string listsPath = "res://Lists/";
        string maleNamesPath = "Male Names.txt";
        string femaleNamesPath = "Female Names.txt";
        string lastNamesPath = "Last Names.txt";

        public List<string> maleNames = new();
        public List<string> femaleNames = new();
        public List<string> lastNames = new();

        [ExportGroup("This is some bullshit.")]
        [Export] public bool isInsideToken;

        public override void _Ready()
        {
            Instance = this;
            LoadNames();
        }

        private void LoadNames()
        {
            //maleFirstNames = null;
            //femaleFirstNames = null;
            //lastNames = null;

            // Load all the names from disk.
            using var directory = DirAccess.Open(listsPath);
            if (directory.DirExists(listsPath))
            {
                using var maleFile = FileAccess.Open(listsPath + maleNamesPath, FileAccess.ModeFlags.Read);
                while(maleFile.GetPosition() < maleFile.GetLength())
                {
                    maleNames.Add(maleFile.GetLine());
                    //GD.Print("First Name: " + maleNames[^1]);
                }
                using var femaleFile = FileAccess.Open(listsPath + femaleNamesPath, FileAccess.ModeFlags.Read);
                while (femaleFile.GetPosition() < femaleFile.GetLength())
                {
                    femaleNames.Add(femaleFile.GetLine());
                    //GD.Print("First Name: " + femaleNames[^1]);
                }
                using var lastNameFile = FileAccess.Open(listsPath + lastNamesPath, FileAccess.ModeFlags.Read);
                while (lastNameFile.GetPosition() < lastNameFile.GetLength())
                {
                    lastNames.Add(lastNameFile.GetLine());
                    //GD.Print("Last Name: " + lastNames[^1]);
                }
            }
            else
            {
                GD.Print("Directory doesn't exist!");
            }
        }
    }
}


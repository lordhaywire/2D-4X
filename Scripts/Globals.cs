using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PlayerSpace
{
    public partial class Globals : Node
    {
        public static Globals Instance { get; private set; }
        public Random random = new();

        [Export] public Godot.Collections.Array<FactionData> factionDatas = [];

        [ExportGroup("Event Variables")]
        [Export] public int dailyInfluenceGain;

        [ExportGroup("Game Settings")]
        [Export] public bool startPaused;

        [ExportGroup("Selected Items")]
        [Export] public int selectedCountyId;
        [Export] private County currentlySelectedCounty;

        public County CurrentlySelectedCounty
        {
            get { return currentlySelectedCounty; }
            set
            {
                if (currentlySelectedCounty != null)
                {
                    currentlySelectedCounty.maskSprite.SelfModulate = new Color(1, 1, 1, 1f);
                }
                currentlySelectedCounty = value;
                currentlySelectedCounty.maskSprite.SelfModulate = new Color(0, 0, 0, 1f);
            }
        }
        [Export] public County selectedLeftClickCounty;
        [Export] public County selectedRightClickCounty;
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
        [Export] public int moraleRecoveryMin = 1;
        [Export] public int moraleRecoveryMax = 11; // One above max.
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

        public List<FactionData> deadFactions = [];

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

        public class ListWithNotify<T> : IEnumerable<T> where T : class
        {
            readonly List<T> list = [];

            public delegate void ItemAddedEventHandler(object sender, T item);

            public event ItemAddedEventHandler ItemAdded;
            // This makes this function like a normal list.
            public T this[int i]
            {
                get
                {
                    if (list.Count != 0)
                    {
                        return list[i];
                    }
                    else
                    {
                        Console.WriteLine("Default: " + default(T));
                        return null;
                    }
                }
                set { list[i] = value; }
            }

            // These makes the normal list actions work correctly. If we need to do other things to the list we need to add them here.
            public int Count() // This is not a normal list so when using it, it will need ().
            {
                return list.Count;
            }
            public void Add(T item)
            {
                list.Add(item);
                OnItemAdded(item);
            }

            public void Insert(int i, T item)
            {
                list.Insert(i, item);
                OnItemAdded(item);
            }

            public void Remove(T item)
            {
                list.Remove(item);
            }

            public void RemoveAt(int i)
            {
                list.RemoveAt(i);
            }

            protected virtual void OnItemAdded(T item)
            {
                ItemAdded?.Invoke(this, item);
            }

            // Implementing IEnumerable<T> interface to enable foreach
            public IEnumerator<T> GetEnumerator()
            {
                return list.GetEnumerator();
            }

            // Implementing IEnumerable interface
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public void AddToFactionHeroList(CountyPopulation countyPopulation)
        {
            // We need to double check that the hero isn't already in the list.
            if (!countyPopulation.factionData.allHeroesList.Contains(countyPopulation))
            {
                countyPopulation.factionData.allHeroesList.Add(countyPopulation);
                GD.Print($"Add To {countyPopulation.factionData.factionName} Hero List: " + countyPopulation.lastName);
            }
        }
        public void OnMouseEnteredUI()
        {
            PlayerControls.Instance.stopClickThrough = true;
            //GD.Print("Mouse Over UI: " + PlayerControls.Instance.stopClickThrough);
        }

        public void OnMouseExitedUI()
        {
            PlayerControls.Instance.stopClickThrough = false;
            //GD.Print("Mouse Over UI: " + PlayerControls.Instance.stopClickThrough);
        }
    }
}


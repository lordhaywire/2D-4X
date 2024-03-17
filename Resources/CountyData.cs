using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class CountyData : Resource
    {
        public event Action IdleWorkersChanged;

        [ExportGroup("MapEditor")]
        public SelectCounty countyNode;
        [Export] public Color color;
        public Vector2I startMaskPosition; // I think this is the local position....
        [Export] public Vector2I countyOverlayLocalPosition;

        [ExportGroup("County other somethings")]
        [Export] public int countyId;
        [Export] public string countyName;
        [Export] public bool isPlayerCapital; // We need to differentiate between player choosen capitals and AI capitals for generation after player creation.
        [Export] public bool isAICapital;
        //[Export] public AllEnums.Factions faction;
        [Export] public FactionData factionData;

        [Export] public AllEnums.Province province;
        [Export] public AllEnums.Terrain biomePrimary;
        [Export] public AllEnums.Terrain biomeSecondary;
        [Export] public AllEnums.Terrain biomeTertiary;

        public Globals.ListWithNotify<CountyPopulation> countyPopulationList = [];
        public Globals.ListWithNotify<CountyPopulation> herosInCountyList = [];
        public Globals.ListWithNotify<CountyPopulation> armiesInCountyList = [];
        public Globals.ListWithNotify<CountyPopulation> visitingHeroList = [];
        public Globals.ListWithNotify<CountyPopulation> visitingArmyList = [];

        public List<Button> spawnTokenButtons = [];

        public List<CountyImprovementData> allCountyImprovements = [];
        public List<CountyImprovementData> underConstructionCountyImprovements = [];
        public List<Battle> battles = [];

        [Export] public int population;

        // These are used just to pass some data around.  Probably I should find a better way to do this.
        public Texture2D maskTexture;
        public Texture2D mapTexture;

        // We will have to see if this is still used.
        public event Action<bool> CountySelected;

        private bool selected = false;
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                if (selected)
                {
                    OnCountySelected(true);
                }
                else
                {
                    OnCountySelected(false);
                }
            }
        }

        private void OnCountySelected(bool isSelected)
        {
            CountySelected?.Invoke(isSelected);
        }

        private int idleWorkers;

        public int IdleWorkers
        {
            get { return idleWorkers; }
            set
            {
                idleWorkers = value;
                IdleWorkersChanged?.Invoke();
            }
        }
    }
}


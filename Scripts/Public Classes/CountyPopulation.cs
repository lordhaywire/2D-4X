using Godot;
using System.Collections.Generic;

namespace PlayerSpace
{
    public class CountyPopulation
    {
        public FactionData factionData;
        public int location;

        public int lastLocation;
        public int destination;

        [ExportGroup("Info")]
        [Export] public string firstName;
        public string lastName;
        public bool isMale;
        public int age;

        public bool isHero;
        public bool isFactionLeader;
        public bool isAide;
        private bool isArmyLeader;
        public bool isWorker;

        public bool IsArmyLeader
        {
            get { return isArmyLeader; }
            set
            {
                isArmyLeader = value;
                if (token != null)
                {
                    AllTokenTextures.Instance.AssignTokenTextures(token);
                }
            }
        }

        [ExportGroup("Perks")]
        public List<PerkData> perks;

        [ExportGroup("Token Expendables")]
        public int moraleExpendable; // I think we are going to have to have this as leader morale or army morale or some shit.
        public int loyaltyAttribute; // I think this needs to be renamed to an expendable.

        [ExportGroup("Attributes")]
        public Godot.Collections.Dictionary<AllEnums.Attributes, AttributeData> attributes = [];

        [ExportGroup("Skills")]
        public Godot.Collections.Dictionary<AllEnums.Skills, SkillData> skills = [];
        public SkillData preferredSkill;

        [ExportGroup("Work")]
        public string currentActivity;
        public string nextActivity;
        private CountyImprovementData currentWork;
        public CountyImprovementData CurrentWork
        {
            get { return currentWork; }
            set
            {
                Activities activities = new();
                currentWork = value;
                if (currentWork == null)
                {
                    activities.UpdateNext(this, AllText.Activities.IDLE);
                }
                else
                {
                    activities.UpdateNext(this, AllText.Activities.WORKING);
                }
            }
        }
        private CountyImprovementData nextWork;
        public CountyImprovementData NextWork
        {
            get { return nextWork; }
            set
            {
                Activities activities = new();
                nextWork = value;
                if (nextWork == null)
                {
                    activities.UpdateNext(this, AllText.Activities.IDLE);
                }
                else
                {
                    activities.UpdateNext(this, AllText.Activities.WORKING);
                }
            }
        }

        private CountyImprovementData currentConstruction; // Building that day.
        public CountyImprovementData CurrentConstruction // Building the next day.
        {
            get { return currentConstruction; }
            set
            {
                Activities activities = new();
                currentConstruction = value;
                if (currentConstruction == null)
                {
                    activities.UpdateNext(this, AllText.Activities.IDLE);
                }
                else
                {
                    activities.UpdateNext(this, AllText.Activities.BUILDING);
                }
            }
        }

        private CountyImprovementData nextContruction;
        public CountyImprovementData NextConstruction // Building the next day.
        {
            get { return nextContruction; }
            set
            {
                Activities activities = new();
                nextContruction = value;
                if (nextContruction == null)
                {
                    activities.UpdateNext(this, AllText.Activities.IDLE);
                }
                else
                {
                    activities.UpdateNext(this, AllText.Activities.BUILDING);
                }
            }
        }

        private ResearchItemData currentResearchItemData;

        public ResearchItemData CurrentResearchItemData
        {
            get { return currentResearchItemData; }
            set
            {
                currentResearchItemData = value;
                if (currentResearchItemData == null)
                {
                    // Change to use Activies public class.
                    nextActivity = AllText.Activities.IDLE;
                    if (CountyInfoControl.Instance?.Visible == true)
                    {
                        CountyInfoControl.Instance.GenerateHeroesPanelList();
                    }
                }
                else
                {
                    nextActivity = AllText.Activities.RESEARCHING;
                }
                if (ResearchControl.Instance?.Visible == true)
                {
                    //GD.Print("It got to the Visiblity Check on Current Research Item Data.");
                    ResearchControl.Instance.CheckForResearchers();
                }
            }
        }

        [ExportGroup("Token")]
        public SelectToken token;

        // They always need to be a hero first for everything else to work.
        public void ChangeToArmy()
        {
            isHero = true;
            IsArmyLeader = true;
            County selectCounty = (County)Globals.Instance.countiesParent.GetChild(location);
            selectCounty.countyData.armiesInCountyList.Add(this);
            selectCounty.countyData.herosInCountyList.Remove(this);
        }

        public CountyPopulation(
            FactionData factionData, int location, int lastLocation, int destination, string firstName, string lastName
            , bool isMale, int age, bool isHero, bool isFactionLeader, bool isAide, bool IsArmyLeader, bool isWorker
            , List<PerkData> perks, int moraleExpendable, int loyaltyAttribute
            , Godot.Collections.Dictionary<AllEnums.Attributes, AttributeData> attributes
            , Godot.Collections.Dictionary<AllEnums.Skills, SkillData> skills
            , SkillData preferredSkill, string currentActivity, string nextActivity, CountyImprovementData CurrentWork
            , CountyImprovementData NextWork
            , CountyImprovementData CurrentConstruction, CountyImprovementData NextConstruction
            , ResearchItemData CurrentResearchItemData)
        {
            this.factionData = factionData;
            this.location = location;
            this.lastLocation = lastLocation;
            this.destination = destination;
            this.firstName = firstName;
            this.lastName = lastName;
            this.isMale = isMale;
            this.age = age;

            this.isHero = isHero;
            this.isFactionLeader = isFactionLeader;
            this.isAide = isAide;
            this.IsArmyLeader = IsArmyLeader;
            this.isWorker = isWorker;

            this.perks = perks;

            this.moraleExpendable = moraleExpendable;
            this.loyaltyAttribute = loyaltyAttribute;
            this.attributes = attributes;

            this.skills = skills;
            this.preferredSkill = preferredSkill;

            this.currentActivity = currentActivity;
            this.nextActivity = nextActivity;
            this.CurrentWork = CurrentWork;
            this.NextWork = NextWork;
            this.CurrentConstruction = CurrentConstruction;
            this.NextConstruction = NextConstruction;
            this.CurrentResearchItemData = CurrentResearchItemData;
        }
    }
}

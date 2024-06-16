using Godot;
using System;

namespace PlayerSpace
{
    public class CountyPopulation
    {
        readonly Activities activities = new();

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
        public Godot.Collections.Dictionary<AllEnums.Perks, PerkData> perks;

        [ExportGroup("Expendables")]
        public int moraleExpendable; // I think we are going to have to have this as leader morale or army morale or some shit.
        private readonly int loyaltyBase;
        private int loyaltyAdjusted;
        public int LoyaltyAdjusted
        {
            get { return loyaltyAdjusted; }
            set
            {
                // Make it so that loyaltyAdjusted can't go above 100.
                loyaltyAdjusted = Math.Min(value, 100);
            }
        }
        private int happiness;

        public int Happiness
        {
            get { return happiness; }
            set 
            {
                happiness = Math.Min(value, 100);
                // Visual Studio says I can do this...Meaning I don't need the new AttributeData.
                LoyaltyAdjusted = loyaltyBase + AttributeData.ApplyAttributeBonuses(value);
                GD.Print($"{firstName} {lastName} loyalty adjusted: {LoyaltyAdjusted}");
            }
        }
        [ExportGroup("Attributes")]
        public Godot.Collections.Dictionary<AllEnums.Attributes, AttributeData> attributes = [];

        [ExportGroup("Skills")]
        public Godot.Collections.Dictionary<AllEnums.Skills, SkillData> skills = [];
        public SkillData preferredSkill;

        [ExportGroup("Work")]
        public AllEnums.Activities currentActivity;
        public AllEnums.Activities nextActivity;
        private CountyImprovementData currentWork;

        // I don't think this needs to be a getter setter because we never set the current construction.  The PopulationAI
        // script does that.
        public CountyImprovementData CurrentWork
        {
            get { return currentWork; }
            set
            {
                currentWork = value;
            }
        }
        private CountyImprovementData nextWork;
        public CountyImprovementData NextWork
        {
            get { return nextWork; }
            set
            {
                nextWork = value;
                if (nextWork == null)
                {
                    activities.UpdateNext(this, AllEnums.Activities.Idle);
                }
                else
                {
                    activities.UpdateNext(this, AllEnums.Activities.Work);
                }
            }
        }

        private CountyImprovementData currentConstruction; // Building that day.

        // I don't think this needs to be a getter setter because we never set the current construction.  The PopulationAI
        // script does that.
        public CountyImprovementData CurrentConstruction 
        {
            get { return currentConstruction; }
            set
            {
                currentConstruction = value;
            }
        }

        private CountyImprovementData nextContruction;
        public CountyImprovementData NextConstruction // Building the next day.
        {
            get { return nextContruction; }
            set
            {
                nextContruction = value;
                if (nextContruction == null)
                {
                    activities.UpdateNext(this, AllEnums.Activities.Idle);
                }
                else
                {
                    activities.UpdateNext(this, AllEnums.Activities.Build);
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
                    activities.UpdateNext(this, AllEnums.Activities.Idle);
                    if (CountyInfoControl.Instance?.Visible == true)
                    {
                        CountyInfoControl.Instance.GenerateHeroesPanelList();
                    }
                }
                else
                {
                    activities.UpdateNext(this, AllEnums.Activities.Research);
                }
                
                if (ResearchControl.Instance?.Visible == true)
                {
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

        public void AddRandomHappiness()
        {
            Random random = new();
            Happiness += random.Next(0, 4);
            GD.Print($"{firstName} {lastName} happiness is now {Happiness}");
        }
        public CountyPopulation(
            FactionData factionData, int location, int lastLocation, int destination, string firstName, string lastName
            , bool isMale, int age, bool isHero, bool isFactionLeader, bool isAide, bool IsArmyLeader, bool isWorker
            , Godot.Collections.Dictionary<AllEnums.Perks, PerkData> perks, int moraleExpendable
            , int loyaltyBase, int loyaltyAdjusted, int Happiness
            , Godot.Collections.Dictionary<AllEnums.Attributes, AttributeData> attributes
            , Godot.Collections.Dictionary<AllEnums.Skills, SkillData> skills
            , SkillData preferredSkill, AllEnums.Activities currentActivity, AllEnums.Activities nextActivity
            , CountyImprovementData CurrentWork
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
            this.loyaltyBase = loyaltyBase;
            this.LoyaltyAdjusted = loyaltyAdjusted;
            this.Happiness = Happiness;
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

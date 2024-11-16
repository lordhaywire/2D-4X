using Godot;
using System;

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
        public Godot.Collections.Dictionary<AllEnums.Perks, PerkData> perks;

        [ExportGroup("Expendables")]
        public int hitpoints;
        public int maxHitpoints;

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
                LoyaltyAdjusted = loyaltyBase + AttributeData.ApplyAttributeBonuses(value, false, false);
                //GD.Print($"{firstName} {lastName} loyalty adjusted: {LoyaltyAdjusted}");
            }
        }

        public int daysStarving;

        // Resource needs, currently there is just 1 need, Remnants.
        public Godot.Collections.Dictionary<AllEnums.CountyGoodType, int> needs;

        [ExportGroup("Attributes")]
        public Godot.Collections.Dictionary<AllEnums.Attributes, AttributeData> attributes = [];

        [ExportGroup("Skills")]
        public Godot.Collections.Dictionary<AllEnums.Skills, SkillData> skills = [];
        public SkillData preferredSkill;
        public InterestData interest;

        [ExportGroup("Work")]
        public AllEnums.Activities activity;
        public CountyImprovementData currentCountyImprovement;

        public ResearchItemData currentResearchItemData;
        
        [ExportGroup("Token")]
        public SelectToken token;

        public bool CheckForPerk(AllEnums.Perks perk)
        {
            if (perks.ContainsKey(perk))
            {
                return true;
            }
            return false;
        }

        // They always need to be a hero first for everything else to work.
        public void ChangeToArmy()
        {
            isHero = true;
            IsArmyLeader = true;
            County selectCounty = (County)Globals.Instance.countiesParent.GetChild(location);
            selectCounty.countyData.armiesInCountyList.Add(this);
            selectCounty.countyData.herosInCountyList.Remove(this);
        }
        public void UpdateActivity(AllEnums.Activities activity)
        {
            this.activity = activity;
        }

        public void UpdateDestination(int destination)
        {
            this.destination = destination;
        }
        public void UpdateCurrentCountyImprovement(CountyImprovementData countyImprovementData)
        {
            currentCountyImprovement = countyImprovementData;
        }

        public void AddRandomHappiness(int maxHappiness)
        {
            Random random = new();
            Happiness += random.Next(1, maxHappiness);
            //GD.Print($"Happiness gained: {firstName} {lastName} happiness is now {Happiness}");
        }

        public string GetActivityName()
        {
            string name = TranslationServer.Translate(AllActivities.Instance.allActivityData[(int)activity].name);
            return name;
        }

        public void RemoveRandomHappiness(int maxHappiness)
        {
            Random random = new();
            Happiness -= random.Next(1, maxHappiness);
            //GD.Print($"Happiness lost: {firstName} {lastName} happiness is now {Happiness}");
        }

        public bool CheckWillWorkLoyalty()
        {
            if (LoyaltyAdjusted >= Globals.Instance.willWorkLoyalty)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Makes activity idle and removes the population from the populationAtImprovement list.
        /// </summary>
        public void RemoveFromCountyImprovement()
        {
            UpdateActivity(AllEnums.Activities.Idle);
            currentCountyImprovement?.populationAtImprovement.Remove(this);
            currentCountyImprovement = null;
        }

        public void UpdateCurrentResearch(ResearchItemData researchItemData)
        {
            UpdateActivity(AllEnums.Activities.Research);
            currentResearchItemData = researchItemData;
        }

        /// <summary>
        /// Removes the populations research Item Data and sets their activity to Work.
        /// </summary>
        public void RemoveFromResearch()
        {
            currentResearchItemData = null;
            UpdateActivity(AllEnums.Activities.Work);
            // When the AI removes people from research it is going to try and do this.  I am not sure we care.
            ResearchControl.Instance.assignedResearchers.Remove(this);
        }

        public CountyPopulation(
            FactionData factionData, int location, int lastLocation, int destination, string firstName, string lastName
            , bool isMale, int age, bool isHero, bool isFactionLeader, bool isAide, bool IsArmyLeader, bool isWorker
            , Godot.Collections.Dictionary<AllEnums.Perks, PerkData> perks, int hitpoints, int maxHitpoints
            , int moraleExpendable
            , int loyaltyBase, int LoyaltyAdjusted, int Happiness, int daysStarving
            , Godot.Collections.Dictionary<AllEnums.CountyGoodType, int> needs
            , Godot.Collections.Dictionary<AllEnums.Attributes, AttributeData> attributes
            , Godot.Collections.Dictionary<AllEnums.Skills, SkillData> skills
            , SkillData preferredSkill, InterestData interest, AllEnums.Activities activity
            , CountyImprovementData currentCountyImprovement
            , ResearchItemData currentResearchItemData)
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

            this.hitpoints = hitpoints;
            this.maxHitpoints = maxHitpoints;
            this.moraleExpendable = moraleExpendable;
            this.loyaltyBase = loyaltyBase;
            this.LoyaltyAdjusted = LoyaltyAdjusted;
            this.Happiness = Happiness;
            this.daysStarving = daysStarving;
            this.needs = needs;
            this.attributes = attributes;

            this.skills = skills;
            this.preferredSkill = preferredSkill;
            this.interest = interest;

            this.activity = activity;
            this.currentCountyImprovement = currentCountyImprovement;
            this.currentResearchItemData = currentResearchItemData;
        }
    }
}

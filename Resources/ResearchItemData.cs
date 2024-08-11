using Godot;

namespace PlayerSpace
{
    [GlobalClass]
    public partial class ResearchItemData : Resource
    {
        [Export] public AllEnums.ResearchTiers tier; // We might not need this now that the research is manually added to the research panel.
        [Export] public AllEnums.Skills skill;
        [Export] public InterestData interest;
        [Export] public bool researchedAtStart;
        [Export] public string researchName;
        [Export] public string researchDescription;
        [Export] public Texture2D researchTexture;

        [Export] private int amountOfResearchDone;
        public int AmountOfResearchDone
        {
            get { return amountOfResearchDone; }
            set
            {
                amountOfResearchDone = value;
                if (amountOfResearchDone >= costOfResearch)
                {
                    amountOfResearchDone = costOfResearch;
                    isResearchDone = true;
                    /*
                    if(Clock.Instance.days > 0 || Clock.Instance.Hours > 1)
                    {
                        GD.Print("I think this will happen too soon.");
                    }
                    */
                }
                else
                {
                    isResearchDone = false;
                }
            }
        }
        [Export] public int costOfResearch;
        // This is the list of countyImprovementDatas that is research controls.
        [Export] public CountyImprovementData[] countyImprovementDatas; 

        // Are we doing something with this?
        public CountyPopulation[] researchers;

        [Export] public bool isResearchDone; // We might not need this because we could just compare the cost vs the amount done.
    
        public void CompleteResearch(FactionData factionData)
        {
            foreach (CountyImprovementData countyImprovementData in countyImprovementDatas)
            {
                factionData.AddCountyImprovementToAllCountyImprovements(countyImprovementData);
            }
        }
    }
}
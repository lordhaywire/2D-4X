using Godot;

namespace PlayerSpace;

[GlobalClass]
public partial class ResearchItemData : Resource
{
    public int factionID;
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
            //GD.PrintRich($"[rainbow]Research Item Data value set.");
            if (amountOfResearchDone >= costOfResearch)
            {
                amountOfResearchDone = costOfResearch;
                //GD.PrintRich($"[rainbow]Research Item Data amount of research is larger then cost.");
                isResearchDone = true;
                // This is going to complete the research when faction generation happens, but it
                // actually doesn't do anything until after the county generation happens.
                // We could probably have it not run until after the game starts.
                CompleteResearch();
            }
            else
            {
                isResearchDone = false;
            }
        }
    }
    [Export] public int costOfResearch;
    // This is the list of countyImprovementDatas that is research controls.
    [Export] public CountyImprovementData[] countyImprovementDatas = [];

    // Are we doing something with this?
    //public CountyPopulation[] researchers;

    [Export] public bool isResearchDone; // We might not need this because we could just compare the cost vs the amount done.

    public void CompleteResearch()
    {
        GD.PrintRich($"[rainbow]Complete Research!");
        EventLog.Instance?.AddLog($"{researchName} has been completed research.");
        GD.Print("County Improvement Array Count: " + countyImprovementDatas.Length);
        if (countyImprovementDatas.Length > 0)
        {
            foreach (CountyImprovementData countyImprovementData in countyImprovementDatas)
            {
                FactionData factionData = FactionData.GetFactionDataFromID(factionID);
                GD.Print($"This is where it breaks: {factionData.factionName} {countyImprovementData.improvementName}");
                factionData.AddCountyImprovementToAllCountyImprovements(countyImprovementData);
            }
        }
    }
    
    public static ResearchItemData NewCopy(ResearchItemData researchItemData)
    {
        ResearchItemData newResearchItemData = new()
        {
            factionID = researchItemData.factionID,
            skill = researchItemData.skill,
            interest = researchItemData.interest,
            researchedAtStart = researchItemData.researchedAtStart,
            researchName = researchItemData.researchName,
            researchDescription = researchItemData.researchDescription,
            researchTexture = researchItemData.researchTexture,
            AmountOfResearchDone = researchItemData.AmountOfResearchDone,
            costOfResearch = researchItemData.costOfResearch,
            countyImprovementDatas = researchItemData.countyImprovementDatas,
            //researchers = researchItemData.researchers,
            isResearchDone = researchItemData.isResearchDone
        };
        return newResearchItemData;
    }
    
}
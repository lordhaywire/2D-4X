using Godot;

namespace PlayerSpace;

[GlobalClass]
public partial class ResearchItemData : Resource
{
    public FactionData factionData;
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
    [Export] public CountyImprovementData[] countyImprovementDatas;

    // Are we doing something with this?
    public CountyPopulation[] researchers;

    [Export] public bool isResearchDone; // We might not need this because we could just compare the cost vs the amount done.

    public void CompleteResearch()
    {
        GD.PrintRich($"[rainbow]Complete Research!");
        if (countyImprovementDatas.Length > 0)
        {
            foreach (CountyImprovementData countyImprovementData in countyImprovementDatas)
            {
                GD.PrintRich($"[rainbow]{countyImprovementData.improvementName} has been added!");
                GD.Print($"This is where it breaks: {factionData.factionName}");
                factionData.AddCountyImprovementToAllCountyImprovements(countyImprovementData);
            }
        }
    }

    /*
    public static ResearchItemData NewCopy()
    {
        ResearchItemData newResearchItemData = new();
        foreach (AttributeData attributeData in AllAttributes.Instance.allAttributes)
        {
            newAttributes.Add(attributeData.attribute, new AttributeData
            {
                attribute = attributeData.attribute,
                attributeName = attributeData.attributeName,
                attributeAbbreviation = attributeData.attributeAbbreviation,
                attributeDescription = attributeData.attributeDescription,
                attributeLevel = attributeData.attributeLevel,
            });
        }
        return newAttributes;
    }
    */
}
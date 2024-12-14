using Godot;
using System;
using System.Collections.Generic;

namespace PlayerSpace;

[GlobalClass]
public partial class ResearchItemData : Resource
{
    public int factionID;
    [Export] public AllEnums.ResearchTiers tier; // We might not need this now that the research is manually added to the research panel.
    [Export] public AllEnums.Skills skill;
    [Export] public InterestData interestData;
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
            amountOfResearchDone = Math.Min(value, costOfResearch);
            //GD.PrintRich($"[rainbow]Research Item Data value set.");
            if (CheckIfResearchDone() == true)
            {
                amountOfResearchDone = costOfResearch;
                //GD.PrintRich($"[rainbow]Research Item Data amount of research is larger then cost.");
                // This is going to complete the research when faction generation happens, but it
                // actually doesn't do anything until after the county generation happens.
                // We could probably have it not run until after the game starts.
            }
        }
    }
    [Export] public int costOfResearch;
    // This is the list of countyImprovementDatas that is research controls.
    [Export] public CountyImprovementData[] countyImprovementDatas = [];
    [Export] public Godot.Collections.Array<EnumsResearch.All> researchPrerequisites;

    public void CompleteResearch()
    {
        GD.PrintRich($"[rainbow]Complete Research! " + researchName);
        Faction faction = (Faction)Globals.Instance.factionsParent.GetChild(factionID);
        if (faction.factionData == Globals.Instance.playerFactionData)
        {
            EventLog.Instance?.AddLog($"{Tr("PHRASE_RESEARCH_FOR")} {Tr(researchName)} {Tr("PHRASE_HAS_BEEN_COMPLETED")}.");
        }
        //GD.Print("County Improvement Array Count: " + countyImprovementDatas.Length);
        if (countyImprovementDatas.Length > 0)
        {
            foreach (CountyImprovementData countyImprovementData in countyImprovementDatas)
            {
                FactionData factionData = FactionData.GetFactionDataFromID(factionID);
                //GD.Print($"This is where it breaks: {factionData.factionName} {countyImprovementData.improvementName}");

                // This is to set the starting adjusted max builders and workers.
                countyImprovementData.adjustedMaxBuilders = countyImprovementData.maxBuilders;
                countyImprovementData.adjustedMaxWorkers = countyImprovementData.maxWorkers;

                factionData.AddCountyImprovementToAllCountyImprovements(countyImprovementData);
            }
        }
    }

    public bool CheckIfResearchDone()
    {
        return amountOfResearchDone >= costOfResearch;
    }

    public bool CheckIfPrerequisitesAreDone()
    {
        foreach (EnumsResearch.All enumResearch in researchPrerequisites)
        {
            //GD.Print($"{Globals.Instance.playerFactionData.researchItems[(int)enumResearch].researchName} " +
            //    $"{Globals.Instance.playerFactionData.researchItems[(int)enumResearch].CheckIfResearchDone()}");
            if (Globals.Instance.playerFactionData.researchItems[(int)enumResearch].CheckIfResearchDone()
                == false)
            {
                return false;
            }
        }
        return true;
    }
    public ResearchItemData NewCopy(ResearchItemData researchItemData)
    {
        ResearchItemData newResearchItemData = new()
        {
            factionID = researchItemData.factionID,
            tier = researchItemData.tier,
            skill = researchItemData.skill,
            interestData = researchItemData.interestData,
            researchedAtStart = researchItemData.researchedAtStart,
            researchName = researchItemData.researchName,
            researchDescription = researchItemData.researchDescription,
            researchTexture = researchItemData.researchTexture,
            AmountOfResearchDone = researchItemData.AmountOfResearchDone,
            costOfResearch = researchItemData.costOfResearch,
            countyImprovementDatas = researchItemData.countyImprovementDatas,
            researchPrerequisites = researchItemData.researchPrerequisites,
        };
        // This was an attempt at deep copying the array.
        //countyImprovementDatas = new CountyImprovementData[researchItemData.countyImprovementDatas.Length],

        /*
        for (int i = 0; i < researchItemData.countyImprovementDatas.Length; i++)
        {
            countyImprovementDatas[i] = CountyImprovementData.NewCopy(researchItemData.countyImprovementDatas[i]);
            GD.Print($"County Improvement: {countyImprovementDatas[i].improvementName}.");
        }
        */

        return newResearchItemData;
    }
    
}
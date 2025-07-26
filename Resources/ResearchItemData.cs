using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoloadSpace;

namespace PlayerSpace;

[GlobalClass]
public partial class ResearchItemData : Resource
{
    public int factionId;
    [Export] public AllEnums.ResearchTiers tier;
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


    public ResearchItemDto ToDto()
    {
        return new ResearchItemDto
        {
            ResearchName = researchName,
            FactionId = factionId,
            AmountOfResearchDone = AmountOfResearchDone
        };
    }

    public static ResearchItemData FromDto(ResearchItemDto dto)
    {
        // Look up the base template from the master list
        ResearchItemData baseItem = Autoload.Instance.allResearchItemData
            .FirstOrDefault(r => r.researchName == dto.ResearchName);

        if (baseItem == null)
        {
            GD.PrintErr($"[ERROR] Could not find ResearchItemData for {dto.ResearchName}");
            return null;
        }

        // Make a copy so we don’t overwrite the master data
        ResearchItemData copy = baseItem.NewCopy(baseItem);

        // Apply unique save data
        copy.factionId = dto.FactionId;
        copy.AmountOfResearchDone = dto.AmountOfResearchDone;

        return copy;
    }


    public void CompleteResearch()
    {
        GD.PrintRich($"[rainbow]Complete Research! " + researchName);
        Faction faction = (Faction)Globals.Instance.factionsParent.GetChild(factionId);
        if (faction.factionData == Autoload.Instance.playerFactionData)
        {
            EventLog.Instance?.AddLog(
                $"{Tr("PHRASE_RESEARCH_FOR")} {Tr(researchName)} {Tr("PHRASE_HAS_BEEN_COMPLETED")}.");
        }

        //GD.Print("County Improvement Array Count: " + countyImprovementDatas.Length);
        if (countyImprovementDatas.Length > 0)
        {
            foreach (CountyImprovementData countyImprovementData in countyImprovementDatas)
            {
                FactionData factionData = FactionData.GetFactionDataFromId(factionId);
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
            if (Autoload.Instance.playerFactionData.researchItems[(int)enumResearch].CheckIfResearchDone()
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
            factionId = researchItemData.factionId,
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
        return newResearchItemData;
    }
}
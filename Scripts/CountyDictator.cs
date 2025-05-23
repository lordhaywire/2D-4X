using Godot;

namespace PlayerSpace;

public partial class CountyDictator : Node
{
    public static CountyDictator Instance { get; private set; }
    public override void _Ready()
    {
        Instance = this;
    }

    public void CaptureCounty(int capturedCountyID, FactionData winnersFactionData)
    {
        County selectCounty = (County)Globals.Instance.countiesParent.GetChild(capturedCountyID);
        //GD.Print("County captured! " + selectCounty.countyData.countyName);

        // Remove county from the counties faction data county list.
        selectCounty.countyData.factionData.countiesFactionOwns.Remove(selectCounty.countyData);

        // If the faction now has 0 counties it needs to be destroyed.
        //GD.Print($"Faction County Count:" + selectCounty.countyData.factionData.countiesFactionOwns.Count);
        if (selectCounty.countyData.factionData.countiesFactionOwns.Count == 0)
        {
            //GD.Print("Capture County before Destroy Faction!");
            DestroyFaction(selectCounty);
        }

        // Go through all the population in that county and assign them the winners faction.
        selectCounty.countyData.factionData = winnersFactionData;
        selectCounty.countySprite.SelfModulate = winnersFactionData.factionColor;
        foreach (PopulationData populationData in selectCounty.countyData.populationDataList)
        {
            populationData.factionData = winnersFactionData;
        }

        // Assign the faction's
    }

    private static void DestroyFaction(County county)
    {
        //GD.Print("All Heroes List Count: " + selectCounty.countyData.factionData.allHeroesList.Count);
        // Remove all of this factions heroes from game.
        FactionCountyPopulationDestroyer(county.countyData.factionData.allHeroesList);

        Globals.Instance.deadFactions.Add(county.countyData.factionData);
        Globals.Instance.allFactionData.Remove(county.countyData.factionData);
        Globals.Instance.factionsParent.GetChild(county.countyData.factionData.factionId).QueueFree();
    }

    // Maybe move to a Faction Dictator script.
    private static void FactionCountyPopulationDestroyer(Godot.Collections.Array<PopulationData> allHeroesList)
    {
        foreach (PopulationData populationData in allHeroesList)
        {
            County selectCounty = (County)Globals.Instance.countiesParent.GetChild(populationData.location);
            selectCounty.countyData.heroesInCountyList.Remove(populationData);
            selectCounty.countyData.armiesInCountyList.Remove(populationData);
            selectCounty.countyData.spawnedTokenButtons.Remove(populationData.heroToken.spawnedTokenButton);
            populationData.heroToken.spawnedTokenButton.QueueFree();
            populationData.heroToken.QueueFree();

            //GD.PrintRich($"[rainbow]{populationData.firstName}");
            CountyInfoControl.Instance.GenerateHeroesPanelList();
        }
        allHeroesList.Clear();
    }
}
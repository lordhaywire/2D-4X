using Godot;

namespace PlayerSpace;

public partial class CountyDictator : Node
{
    public static CountyDictator Instance { get; private set; }
    public override void _Ready()
    {
        Instance = this;
    }

    public void CaptureCounty(int capturedCountyId, FactionData winnersFactionData)
    {
        County selectCounty = (County)Globals.Instance.countiesParent.GetChild(capturedCountyId);
        //GD.Print("County captured! " + selectCounty.countyData.countyName);

        // Remove county from the counties' faction data county list.
        selectCounty.countyData.factionData.countiesFactionOwns.Remove(selectCounty.countyData);

        // If the faction now has 0 counties, it needs to be destroyed.
        //GD.Print($"Faction County Count: " + selectCounty.countyData.factionData.countiesFactionOwns.Count);
        if (selectCounty.countyData.factionData.countiesFactionOwns.Count == 0)
        {
            //GD.Print("Capture County before Destroy Faction!");
            DestroyFaction(selectCounty);
        }

        // Go through all the population in that county and assign them the winner's faction.
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
        // Remove all of these faction heroes from the game.
        FactionCountyPopulationDestroyer(county.countyData.factionData.allHeroesList);

        // Since we got rid of the allFactionData list, we are now just reparenting the dead faction to the dead faction node.  You can probably delete this comment
        // soon.
        //Globals.Instance.deadFactions.Add(county.countyData.factionData);
        Faction.ConvertFactionToDeadFaction(county.countyData.factionData.factionId);
        //Globals.Instance.factionsParent.GetChild(county.countyData.factionData.factionId).QueueFree();
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
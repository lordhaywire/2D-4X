using System.Collections.Generic;
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
        FactionData factionData = SaveManager.Instance.saveGameData.ConvertFactionIdToFactionData(selectCounty.countyData.factionId);
        factionData.countiesFactionOwns.Remove(selectCounty.countyData);

        // If the faction now has 0 counties, it needs to be destroyed.
        //GD.Print($"Faction County Count: " + selectCounty.countyData.factionData.countiesFactionOwns.Count);
        if (factionData.countiesFactionOwns.Count == 0)
        {
            //GD.Print("Capture County before Destroy Faction!");
            DestroyFaction(selectCounty);
        }

        // Go through all the population in that county and assign them the winner's faction.
        factionData = winnersFactionData;
        selectCounty.countySprite.SelfModulate = winnersFactionData.factionColor;
        foreach (PopulationData populationData in selectCounty.countyData.populationDataList)
        {
            populationData.factionId = winnersFactionData.factionId;
        }

        // Assign the faction's - Todo: What is missing from here? 
    }

    private static void DestroyFaction(County county)
    {
        //GD.Print("All Heroes List Count: " + selectCounty.countyData.factionData.allHeroesList.Count);
        // Remove the destroyed faction heroes from the game.
        FactionData factionData =
            SaveManager.Instance.saveGameData.ConvertFactionIdToFactionData(county.countyData.factionId);
        FactionCountyPopulationDestroyer(factionData.allHeroesDictionary);

        // Since we got rid of the allFactionData list, we are now just reparenting the dead faction to the dead faction node.  You can probably delete this comment
        // soon.
        //Globals.Instance.deadFactions.Add(county.countyData.factionData);
        FactionDictator.ConvertFactionToDeadFaction(factionData.factionId);
        //Globals.Instance.factionsParent.GetChild(county.countyData.factionData.factionId).QueueFree();
    }

    // Maybe move to a Faction Dictator script.
    private static void FactionCountyPopulationDestroyer(Dictionary<int, int> allHeroesDictionary)
    {
        foreach (KeyValuePair<int, int> keyValuePair in allHeroesDictionary)
        {
            County county = (County)Globals.Instance.countiesParent.GetChild(keyValuePair.Value);
            PopulationData hero =
                PopulationData.ReturnPopulationDataFromPopulationId(county.countyData.heroesInCountyList,
                    keyValuePair.Key);
            GD.Print($"FactionCountyPopulationDestroyer - Hero getting removed {hero.GetFullName()}");
            county.countyData.heroesInCountyList.Remove(hero);
            county.countyData.spawnedTokenButtons.Remove(hero.heroToken.spawnedTokenButton);
            hero.heroToken.spawnedTokenButton.QueueFree();
            hero.heroToken.QueueFree();

            //GD.PrintRich($"[rainbow]{populationData.firstName}");
            CountyInfoControl.Instance.GenerateHeroesPanelList();
        }
        allHeroesDictionary.Clear();
    }
}
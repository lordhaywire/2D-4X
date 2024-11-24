using Godot;
using System.Collections.Generic;

namespace PlayerSpace
{
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
            foreach (CountyPopulation countyPopulation in selectCounty.countyData.countyPopulationList)
            {
                countyPopulation.factionData = winnersFactionData;
            }

            // Assign the faction's
        }

        private static void DestroyFaction(County selectCounty)
        {
            //GD.Print("All Heroes List Count: " + selectCounty.countyData.factionData.allHeroesList.Count);
            // Remove all of this factions heroes from game.
            FactionCountyPopulationDestroyer(selectCounty.countyData.factionData.allHeroesList);

            Globals.Instance.deadFactions.Add(selectCounty.countyData.factionData);
            Globals.Instance.factionDatas.Remove(selectCounty.countyData.factionData);
            FactionGeneration.Instance.GetChild(selectCounty.countyData.factionData.factionID).QueueFree();
        }

        // Maybe move to a Faction Dictator script.
        private static void FactionCountyPopulationDestroyer(Godot.Collections.Array<CountyPopulation> allHeroesList)
        {
            foreach (CountyPopulation countyPopulation in allHeroesList)
            {
                County selectCounty = (County)Globals.Instance.countiesParent.GetChild(countyPopulation.location);
                selectCounty.countyData.heroesInCountyList.Remove(countyPopulation);
                selectCounty.countyData.armiesInCountyList.Remove(countyPopulation);
                selectCounty.countyData.spawnedTokenButtons.Remove(countyPopulation.token.spawnedTokenButton);
                countyPopulation.token.spawnedTokenButton.QueueFree();
                countyPopulation.token.QueueFree();

                //GD.PrintRich($"[rainbow]{countyPopulation.firstName}");
                CountyInfoControl.Instance.GenerateHeroesPanelList();
            }
            allHeroesList.Clear();
        }
    }
}
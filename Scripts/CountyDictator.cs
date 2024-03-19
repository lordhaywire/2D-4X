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
            SelectCounty selectCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(capturedCountyID);
            GD.Print("Got to Capture County!" + selectCounty.countyData.countyName);
            selectCounty.countyData.factionData.countiesFactionOwns.Remove(selectCounty.countyData);
            // Go through all the population in that county and assign them the winners faction.

            GD.Print($"Faction County Count:" + selectCounty.countyData.factionData.countiesFactionOwns.Count);
            if(selectCounty.countyData.factionData.countiesFactionOwns.Count == 0)
            {
                GD.Print("Capture County!");
                DestroyFaction(selectCounty);
            }

            selectCounty.countyData.factionData = winnersFactionData;
            selectCounty.countySprite.SelfModulate = winnersFactionData.factionColor;
            foreach (CountyPopulation countyPopulation in selectCounty.countyData.countyPopulationList)
            {
                countyPopulation.factionData = winnersFactionData;
            }
        }

        private void DestroyFaction(SelectCounty selectCounty)
        {
            Globals.Instance.deadFactions.Add(selectCounty.countyData.factionData);
            Globals.Instance.factionDatas.Remove(selectCounty.countyData.factionData);
            FactionGeneration.Instance.GetChild(selectCounty.countyData.factionData.factionID).QueueFree();
            // Remove all of this factions heroes from game.
            CountyPopulationDestroyer(selectCounty.countyData.factionData.allHeroesList);
            // Remove all of this factions armies from the game.


        }

        // Maybe move to a Faction Dictator.
        private void CountyPopulationDestroyer(List<CountyPopulation> countyPopulationList)
        {
            foreach (CountyPopulation countyPopulation in countyPopulationList)
            {
                SelectCounty selectCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(countyPopulation.location);
                selectCounty.countyData.herosInCountyList.Remove(countyPopulation);
                selectCounty.countyData.armiesInCountyList.Remove(countyPopulation);
                countyPopulation.token.spawnedTokenButton.QueueFree();
                countyPopulation.token.QueueFree();

                countyPopulationList.Remove(countyPopulation);
                GD.PrintRich($"[rainbow]{countyPopulation.firstName}");
                CountyInfoControl.Instance.GenerateHeroesPanelList();
            }
        }
    }
}
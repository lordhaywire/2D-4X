using Godot;
using System;

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
            selectCounty.countyData.factionData = winnersFactionData;
            selectCounty.countySprite.SelfModulate = winnersFactionData.factionColor;
            foreach(CountyPopulation countyPopulation in selectCounty.countyData.countyPopulationList)
            {
                countyPopulation.factionData = winnersFactionData;
            }
            GD.Print("Capture County! " + selectCounty.countyData.countyName);
        }
    }
}
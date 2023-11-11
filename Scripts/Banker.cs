using Godot;

namespace PlayerSpace
{
    public partial class Banker : Node
    {
        public static Banker Instance { get; private set; }

        //[SerializeField] private ResourceSO foodSO;

        public override void _Ready()
        {
            Instance = this;
        }

        public void ChargeForHero()
        {
            GD.Print("Player Influence: " + FactionGeneration.Instance.playerFaction.Influence);
            FactionGeneration.Instance.playerFaction.Influence -= Globals.Instance.costOfHero;
        }

        /*
        public void CountIdleWorkers(County county)
        {
            // I don't think this is very efficient.
            int idleWorkers = 0;

            for (int i = 0; i < county.countyPopulation.Count; i++)
            {
                if (county.countyPopulation[i].nextBuilding == null)
                {
                    idleWorkers++;
                }
            }

            county.IdleWorkers = idleWorkers;
        }

        public bool CheckEnoughIdleWorkers(BuildingInfo buildingInfo)
        {
            return buildingInfo.county.IdleWorkers >= buildingInfo.CurrentWorkers;
        }
        public bool CheckForWorkersAssigned(BuildingInfo buildingInfo)
        {
            return buildingInfo.CurrentWorkers > 0;
        }
        public void ChargeForBuilding(Faction faction, BuildingInfo buildingInfo)
        {
            faction.Influence -= buildingInfo.influenceCost;
        }
        public bool CheckBuildingCost(Faction faction, BuildingInfo buildingInfo)
        {
            return faction.Influence >= buildingInfo.influenceCost;
        }

        public bool CheckEnoughFood(Faction faction)
        {
            return faction.food >= Globals.Instance.minimumFoodAI;
        }

        public GameObject FindFoodBuilding(GameObject faction)
        {
            List<County> counties = faction.GetComponent<FactionAI>().countiesFactionOwns;

            for (int i = 0; i < counties.Count; i++)
            {
                Transform possibleBuildingsParent = counties[i].gameObject.GetComponent<CountyInfo>().possibleBuildingsParent;
                Transform currentBuildingsParent = counties[i].gameObject.GetComponent<CountyInfo>().currentBuildingsParent;

                for (int j = 0; j < currentBuildingsParent.childCount; j++)
                {
                    BuildingInfo buildingInfo = currentBuildingsParent.GetChild(j).GetComponent<BuildingInfo>();
                    if (buildingInfo.isBeingBuilt == true || buildingInfo.isBuilt == true)
                    {
                        Debug.Log($"{buildingInfo.buildingName} is already being built.");
                        return null;
                    }
                }

                for (int j = 0; j < possibleBuildingsParent.childCount; j++)
                {
                    BuildingInfo buildingInfo = possibleBuildingsParent.GetChild(j).GetComponent<BuildingInfo>();

                    if (buildingInfo.resourceSO.name == foodSO.name)
                    {
                        GameObject foodBuilding = possibleBuildingsParent.GetChild(j).gameObject;
                        Debug.Log($"Found {foodBuilding.name} in {counties[i].gameObject.name}");

                        return foodBuilding;
                    }
                }
            }
            Debug.Log("No food building found.");
            return null;
        }


        public void DeductCostOfBuilding()
        {
            Debug.Log("Banker.cs DeductCostOfBuilding()");
            /*
            WorldMapLoad.Instance.factions[WorldMapLoad.Instance.playerFactionID].Influence 
                -= WorldMapLoad.Instance.CurrentlySelectedCounty.GetComponent<CountyInfo>().county
                .possibleBuildings[UIPossibleBuildingsPanel.Instance.PossibleBuildingNumber].GetComponent<BuildingInfo>().influenceCost;

        }
        */

    }

}

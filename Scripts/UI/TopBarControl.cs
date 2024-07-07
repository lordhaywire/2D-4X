using Godot;
using System.Collections.Generic;

namespace PlayerSpace
{
    public partial class TopBarControl : Control
    {
        public static TopBarControl Instance { get; private set; }

        [Export] private Clock clock;
        [Export] private Label influenceLabel;
        [Export] private Label influenceAmountUsed;
        [Export] private Label moneyLabel;
        [Export] private Label moneyAmountUsed;
        [Export] private Label remnantsLabel;
        [Export] private Label remnantsAmountUsed;
        [Export] private Label buildingMaterialsLabel;
        [Export] private Label buildingMaterialsAmountUsed;
        [Export] private Label foodLabel;
        [Export] private Label foodAmountUsed;

        public override void _Ready()
        {
            Instance = this;
            UpdateResourceLabels();
        }

        public static void UpdateCountyResources()
        {
            if (Globals.Instance.SelectedLeftClickCounty != null)
            {
                County county = Globals.Instance.SelectedLeftClickCounty;
                // This zeroes the resources so the count is correct when it goes through just 1 counties resources.
                ZeroFactionResources(Globals.Instance.playerFactionData);
                CountFactionResources(Globals.Instance.playerFactionData, county.countyData.countyResources);
            }
        }

        public static void UpdateFactionResources()
        {
            FactionData factionData = Globals.Instance.playerFactionData;
            ZeroFactionResources(factionData);
            foreach(CountyData countyData in factionData.countiesFactionOwns)
            {
                CountFactionResources(Globals.Instance.playerFactionData, countyData.countyResources);
            }
        }

        private static void ZeroFactionResources(FactionData factionData)
        {
            factionData.factionResources[AllEnums.FactionResourceType.Food].amount = 0;
            factionData.factionResources[AllEnums.FactionResourceType.Remnants].amount = 0;
            factionData.factionResources[AllEnums.FactionResourceType.BuildingMaterial].amount = 0;
        }
        private static void CountFactionResources(FactionData factionData, Godot.Collections.Dictionary<AllEnums.CountyResourceType, CountyResourceData> resources)
        {
            foreach (KeyValuePair<AllEnums.CountyResourceType, CountyResourceData> keyValuePair in resources)
            {
                CountyResourceData countyResource = keyValuePair.Value;
                switch (countyResource.factionResourceType)
                {
                    case AllEnums.FactionResourceType.Food:
                        factionData.factionResources[AllEnums.FactionResourceType.Food].amount
                            += countyResource.amount;
                        break;
                    case AllEnums.FactionResourceType.Remnants:
                        factionData.factionResources[AllEnums.FactionResourceType.Remnants].amount
                            += countyResource.amount;
                        break;
                    case AllEnums.FactionResourceType.BuildingMaterial:
                        factionData.factionResources[AllEnums.FactionResourceType.BuildingMaterial].amount
                            += countyResource.amount;
                        break;
                }
            }
        }

        // We should probably make this static somehow so that it matches the other ones.
        public void UpdateResourceLabels()
        {
            GD.Print("Expendables have been updated, motherfucker!");
            FactionData factionData = Globals.Instance.playerFactionData;
            influenceLabel.Text = factionData.factionResources[AllEnums.FactionResourceType.Influence].amount.ToString();
            moneyLabel.Text = factionData.factionResources[AllEnums.FactionResourceType.Money].amount.ToString();
            foodLabel.Text = factionData.factionResources[AllEnums.FactionResourceType.Food].amount.ToString();
            remnantsLabel.Text = factionData.factionResources[AllEnums.FactionResourceType.Remnants].amount.ToString();
            buildingMaterialsLabel.Text = factionData.factionResources[AllEnums.FactionResourceType.BuildingMaterial].amount.ToString();

            // Update the used resource amount.
            influenceAmountUsed.Text
                = $"({factionData.amountUsedFactionResources[AllEnums.FactionResourceType.Influence].amount})";
            moneyAmountUsed.Text
                = $"({factionData.amountUsedFactionResources[AllEnums.FactionResourceType.Money].amount})";
        }

        public void ChangeSpeed(int speed)
        {
            clock.ChangeSpeed(speed);
        }
    }
}
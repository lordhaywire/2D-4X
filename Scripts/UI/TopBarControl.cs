using Godot;

namespace PlayerSpace;

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
    [Export] private Label equipmentLabel;
    [Export] private Label equipmentAmountUsed;
    [Export] private Label foodLabel;
    [Export] private Label foodAmountUsed;
    [Export] private Button x0Button;
    [Export] private Button x1Button;
    [Export] private Button x2Button;
    [Export] private Button x4Button;
    [Export] private Button x8Button;

    FactionData factionData;

    public override void _Ready()
    {
        Instance = this;
        factionData = Globals.Instance.playerFactionData;
        UpdateTopBarGoodLabels();
        CreateSignalsForTimeButtons();
    }

    private void CreateSignalsForTimeButtons()
    {
        x0Button.Pressed += () => Clock.Instance.ChangeSpeed(0);
        x1Button.Pressed += () => Clock.Instance.ChangeSpeed(1);
        x2Button.Pressed += () => Clock.Instance.ChangeSpeed(2);
        x4Button.Pressed += () => Clock.Instance.ChangeSpeed(4);
        x8Button.Pressed += () => Clock.Instance.ChangeSpeed(8);
    }

    public void UpdateTopBarGoodLabels()
    {
        //GD.Print("Top Bar expendables have been updated, motherfucker!");
        UpdateInfluenceMoneyLabels();
        UpdateUsedInfluenceMoneyLabels();

        if (Globals.Instance.SelectedLeftClickCounty == null)
        {
            FactionLevelGoods();
        }
        else
        {
            CountyLevelGoods();
        }
    }

    private void CountyLevelGoods()
    {
        UpdateLabelWithCountyAmount();
        UpdateLabelWithCountyUsedAmount();
    }

    private void UpdateLabelWithCountyAmount()
    {
        CountyData countyData = Globals.Instance.SelectedLeftClickCounty.countyData;

        foodLabel.Text = $"{countyData.CountFactionResourceOfType(AllEnums.FactionGoodType.Food)}";
        remnantsLabel.Text = $"{countyData.CountFactionResourceOfType(AllEnums.FactionGoodType.Remnants)}";
        buildingMaterialsLabel.Text =
            $"{countyData.CountFactionResourceOfType(AllEnums.FactionGoodType.BuildingMaterial)}";
        equipmentLabel.Text = $"{countyData.CountFactionResourceOfType(AllEnums.FactionGoodType.Equipment)}";
    }

    private void UpdateLabelWithCountyUsedAmount()
    {
        CountyData countyData = Globals.Instance.SelectedLeftClickCounty.countyData;

        foodAmountUsed.Text = $"({countyData.CountUsedFactionResourceOfType(AllEnums.FactionGoodType.Food)})";
        remnantsAmountUsed.Text = $"({countyData.CountUsedFactionResourceOfType(AllEnums.FactionGoodType.Remnants)})";
        buildingMaterialsAmountUsed.Text =
            $"({countyData.CountUsedFactionResourceOfType(AllEnums.FactionGoodType.BuildingMaterial)})";
        equipmentAmountUsed.Text = $"({countyData.CountUsedFactionResourceOfType(AllEnums.FactionGoodType.Equipment)})";
    }

    private void UpdateInfluenceMoneyLabels()
    {
        influenceLabel.Text = factionData.factionGoods[AllEnums.FactionGoodType.Influence].Amount.ToString();
        moneyLabel.Text = factionData.factionGoods[AllEnums.FactionGoodType.Money].Amount.ToString();
    }

    private void UpdateUsedInfluenceMoneyLabels()
    {
        influenceAmountUsed.Text
            = $"({factionData.amountUsedFactionGoods[AllEnums.FactionGoodType.Influence].Amount})";
        moneyAmountUsed.Text
            = $"({factionData.amountUsedFactionGoods[AllEnums.FactionGoodType.Money].Amount})";
    }

    private void FactionLevelGoods()
    {
        // Count all the county resources and assign them to the faction resource dictionary.
        factionData.CountAllCountyFactionResources();
        factionData.CountAllCountyFactionUsedResources();

        // Update all of the faction labels
        UpdateLabelsWithFactionAmounts();

        // Update the used resource amount.
        UpdateLabelsWithFactionUsedAmounts();
    }

    private void UpdateLabelsWithFactionUsedAmounts()
    {
        foodAmountUsed.Text = $"({factionData.amountUsedFactionGoods[AllEnums.FactionGoodType.Food].Amount})";
        remnantsAmountUsed.Text = $"({factionData.amountUsedFactionGoods[AllEnums.FactionGoodType.Remnants].Amount})";
        buildingMaterialsAmountUsed.Text =
            $"({factionData.amountUsedFactionGoods[AllEnums.FactionGoodType.BuildingMaterial].Amount})";
        equipmentAmountUsed.Text = $"({factionData.amountUsedFactionGoods[AllEnums.FactionGoodType.Equipment].Amount})";
    }

    private void UpdateLabelsWithFactionAmounts()
    {
        foodLabel.Text = factionData.factionGoods[AllEnums.FactionGoodType.Food].Amount.ToString();
        remnantsLabel.Text = factionData.factionGoods[AllEnums.FactionGoodType.Remnants].Amount.ToString();
        buildingMaterialsLabel.Text =
            factionData.factionGoods[AllEnums.FactionGoodType.BuildingMaterial].Amount.ToString();
        equipmentLabel.Text = factionData.factionGoods[AllEnums.FactionGoodType.Equipment].Amount.ToString();
    }

    public void ChangeSpeed(int speed)
    {
        clock.ChangeSpeed(speed);
    }
}
using Godot;
using System.Collections.Generic;
using AutoloadSpace;

namespace PlayerSpace;

public partial class GoodsPanelContainer : PanelContainer
{
    public static GoodsPanelContainer Instance { get; private set; }

    [Export] private PanelContainer negativeStoragePanelContainer;

    [ExportGroup("Storage Labels")]
    [Export] private Label countyNameTitleLabel;
    [Export] private Label currentPerishableAvailableLabel;
    [Export] private Label maxPerishableAmountAvailableLabel;
    [Export] private Label currentNonperishableAvailableLabel;
    [Export] private Label maxNonperishableAmountAvailableLabel;

    [ExportGroup("Storage HBoxes")]
    [Export] private PackedScene storageHBoxPackedScene;
    [Export] private VBoxContainer perishableVboxParent;
    [Export] private VBoxContainer nonperishableVboxParent;


    private CountyData countyData;
    
    // This one shouldn't be getting the ERROR because it is not exported.
    private Godot.Collections.Dictionary<AllEnums.CountyGoodType, StorageHBox> resourceStorageHBoxDictionary = [];

    public override void _Ready()
    {
        Instance = this;
        GenerateStorageHBoxes();
    }

    // Creates the Storage HBoxes when the game starts.
    private void GenerateStorageHBoxes()
    {
        foreach (GoodData goodData in Autoload.Instance.allGoods)
        {
            if (goodData.goodType == AllEnums.GoodType.FactionGood)
            {
                continue;
            }
            
            StorageHBox storageHBox = (StorageHBox)storageHBoxPackedScene.Instantiate();
            //GD.Print("Resource Name: " + resourceData.name);
            storageHBox.Name = goodData.goodName;
            storageHBox.goodData = goodData; // This is strange.

            if (storageHBox.goodData.perishable == AllEnums.Perishable.Perishable)
            {
                perishableVboxParent.AddChild(storageHBox);
            }
            else if (storageHBox.goodData.perishable == AllEnums.Perishable.Nonperishable)
            {
                nonperishableVboxParent.AddChild(storageHBox);
            }
            resourceStorageHBoxDictionary.Add(storageHBox.goodData.countyGoodType, storageHBox);
        }
    }

    private void OnResourcesPanelContainerVisibilityChanged()
    {
        if (Visible)
        {
            countyData = Globals.Instance.SelectedLeftClickCounty.countyData;
            /* This is just for testing.
            foreach (KeyValuePair<AllEnums.CountyResourceType, ResourceData> keyValuePair in countyData.resources)
            {
                GD.Print($"{keyValuePair.Value.name}: {keyValuePair.Value.MaxAmount}");
            }
            */
            countyNameTitleLabel.Text = countyData.countyName; // We could change this to a method at some point.
            AssignGoodsToStorageHBoxes();
            UpdateMaxAvailableStorageAmountLabels();
            UpdateEachHBoxWithResource();

            Clock.Instance.PauseTime();
            PlayerControls.Instance.playerControlsEnabled = false;
        }
        else
        {
            // Set the county resource max storage to equal whatever the player left it as.
            SetResourceMaxValues(resourceStorageHBoxDictionary);

            Clock.Instance.UnpauseTime();
            PlayerControls.Instance.playerControlsEnabled = true;
        }
    }

    // This assigns the county's resources to the Storage HBoxes when this becomes visible.
    private void AssignGoodsToStorageHBoxes()
    {
        foreach (KeyValuePair<AllEnums.CountyGoodType, GoodData> keyValuePair
                 in countyData.goods)
        {
            resourceStorageHBoxDictionary[keyValuePair.Key].goodData = keyValuePair.Value;
        }
    }

    // Updates top max available amount labels, not the spinbox max amounts.
    private void UpdateMaxAvailableStorageAmountLabels()
    {
        currentPerishableAvailableLabel.Text
            = CountAvailableStorageAmounts(countyData.perishableStorage, AllEnums.Perishable.Perishable).ToString();
        maxPerishableAmountAvailableLabel.Text = countyData.perishableStorage.ToString();

        currentNonperishableAvailableLabel.Text
            = CountAvailableStorageAmounts(countyData.nonperishableStorage, AllEnums.Perishable.Nonperishable).ToString();
        maxNonperishableAmountAvailableLabel.Text = countyData.nonperishableStorage.ToString();
    }

    // Goes through every good and counts how much storage is assigned to them.
    private int CountAvailableStorageAmounts(int maxStorage, AllEnums.Perishable perishable)
    {
        int storage = 0;
        foreach (GoodData goodData in countyData.goods.Values)
        {
            if (goodData.perishable == perishable)
            {
                storage += goodData.MaxAmount;
            }
        }
        int availableStorage = maxStorage - storage;
        return availableStorage;
    }

    // I think the dictionary is in the same order as the resources.
    // Someone in chat told me that there is no order to a dictionary.
    private static void SetResourceMaxValues(Godot.Collections.Dictionary<AllEnums.CountyGoodType, StorageHBox> storageHBoxes)
    {
        foreach (KeyValuePair<AllEnums.CountyGoodType, StorageHBox> keyValuePair in storageHBoxes)
        {
            keyValuePair.Value.goodData.MaxAmount = (int)keyValuePair.Value.maxAmountSpinBox.Value;
        }
    }

    // This one needs to differentiate between perishable and nonperishable.
    // It needs to add the label amount so that the player can add more to the spinbox.
    public void UpdateSpinBoxMaxValuePlusLabel()
    {
        foreach (KeyValuePair<AllEnums.CountyGoodType, StorageHBox> keyValuePair in resourceStorageHBoxDictionary)
        {
            if (keyValuePair.Value.goodData.perishable == AllEnums.Perishable.Perishable)
            {
                keyValuePair.Value.maxAmountSpinBox.MaxValue = keyValuePair.Value.maxAmountSpinBox.Value
                                                               + int.Parse(currentPerishableAvailableLabel.Text);
            }
            else if (keyValuePair.Value.goodData.perishable == AllEnums.Perishable.Nonperishable)
            {
                keyValuePair.Value.maxAmountSpinBox.MaxValue = keyValuePair.Value.maxAmountSpinBox.Value
                                                               + int.Parse(currentNonperishableAvailableLabel.Text);
            }
        }
    }

    public void UpdateCountyAvailableStorageLabels()
    {
        int totalUsedPerishableStorage = 0;
        int totalUsedNonperishableStorage = 0;
        foreach (KeyValuePair<AllEnums.CountyGoodType, StorageHBox> keyValuePair in resourceStorageHBoxDictionary)
        {
            if (keyValuePair.Value.goodData.perishable == AllEnums.Perishable.Perishable)
            {
                totalUsedPerishableStorage += (int)keyValuePair.Value.maxAmountSpinBox.Value;
            }
            else if (keyValuePair.Value.goodData.perishable == AllEnums.Perishable.Nonperishable)
            {
                totalUsedNonperishableStorage += (int)keyValuePair.Value.maxAmountSpinBox.Value;

            }
            int totalAvailablePerishableStorage = countyData.perishableStorage - totalUsedPerishableStorage;
            currentPerishableAvailableLabel.Text = totalAvailablePerishableStorage.ToString();

            int totalAvailableNonperishableStorage = countyData.nonperishableStorage - totalUsedNonperishableStorage;
            currentNonperishableAvailableLabel.Text = totalAvailableNonperishableStorage.ToString();
        }
    }

    private void UpdateEachHBoxWithResource()
    {
        foreach (KeyValuePair<AllEnums.CountyGoodType, StorageHBox> keyValuePair in resourceStorageHBoxDictionary)
        {
            UpdateStorageHBoxLabels(keyValuePair);
            //GD.Print(resourceStorageHBoxDictionary[keyValuePair.Key].resourceData.MaxAmount);
            resourceStorageHBoxDictionary[keyValuePair.Key].maxAmountSpinBox.MaxValue
                = resourceStorageHBoxDictionary[keyValuePair.Key].goodData.MaxAmount;
            resourceStorageHBoxDictionary[keyValuePair.Key].maxAmountSpinBox.Value
                = resourceStorageHBoxDictionary[keyValuePair.Key].goodData.MaxAmount;
        }
    }


    private void UpdateStorageHBoxLabels(KeyValuePair<AllEnums.CountyGoodType, StorageHBox> keyValuePair)
    {
        resourceStorageHBoxDictionary[keyValuePair.Key].goodData = countyData.goods[keyValuePair.Key];
        //GD.Print("Update Storage HBox Labels Resource:" + countyData.resources[keyValuePair.Key].name);
        resourceStorageHBoxDictionary[keyValuePair.Key].goodNameLabel.Text = $"{Tr(resourceStorageHBoxDictionary[keyValuePair.Key].goodData.goodName)}:";
        resourceStorageHBoxDictionary[keyValuePair.Key].goodAmountLabel.Text = resourceStorageHBoxDictionary[keyValuePair.Key].goodData.Amount.ToString();
    }

    private bool IsStoragePositive()
    {
        if(countyData.nonperishableStorage < 0 || countyData.perishableStorage < 0)
        {
            return false;
        }
        return true;
    }

    private void CloseButtonPressed()
    {
        if (IsStoragePositive())
        {
            Hide();
        }
        else
        {
            negativeStoragePanelContainer.Show();
        }
    }

    private void NegativeStoragePanelCloseButtonPressed()
    {
        negativeStoragePanelContainer.Hide();
    }
}
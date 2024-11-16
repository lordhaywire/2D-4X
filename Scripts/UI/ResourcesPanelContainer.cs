using Godot;
using System.Collections.Generic;

namespace PlayerSpace
{
    public partial class ResourcesPanelContainer : PanelContainer
    {
        public static ResourcesPanelContainer Instance { get; private set; }

        [ExportGroup("Storage Labels")]
        [Export] private Label countyNameTitleLabel;
        [Export] private Label currentPerishableAvailableLabel;
        [Export] private Label maxPerishableAmountAvailableLabel;
        [Export] private Label currentNonperishableAvailableLabel;
        [Export] private Label maxNonperisableAmountAvailableLabel;

        [ExportGroup("Storage Hboxes")]
        [Export] private PackedScene storageHBoxPackedScene;
        [Export] private VBoxContainer perishableVboxParent;
        [Export] private VBoxContainer nonperishableVboxParent;

        private CountyData countyData;

        public Godot.Collections.Dictionary<AllEnums.CountyGoodType, StorageHbox> resourceStorageHboxDictionary = [];

        public override void _Ready()
        {
            Instance = this;
            GenerateStorageHBoxes();
        }

        // Creates the Storage Hboxes when the game starts.
        private void GenerateStorageHBoxes()
        {
            foreach (GoodData goodData in AllGoods.Instance.allGoods)
            {
                if (goodData.goodType == AllEnums.GoodType.FactionGood)
                {
                    continue;
                }

                StorageHbox storageHbox = (StorageHbox)storageHBoxPackedScene.Instantiate();
                //GD.Print("Resource Name: " + resourceData.name);
                storageHbox.Name = goodData.goodName;
                storageHbox.goodData = goodData; // This is strange.

                if (storageHbox.goodData.perishable == AllEnums.Perishable.Perishable)
                {
                    perishableVboxParent.AddChild(storageHbox);
                }
                else if(storageHbox.goodData.perishable == AllEnums.Perishable.Nonperishable)
                {
                    nonperishableVboxParent.AddChild(storageHbox);
                }
                resourceStorageHboxDictionary.Add(storageHbox.goodData.countyGoodType, storageHbox);
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
                AssignResourcesToStorageHboxes();
                UpdateMaxAvailableStorageAmountLabels();
                UpdateEachHboxWithResource();

                Clock.Instance.PauseTime();
                PlayerControls.Instance.playerControlsEnabled = false;
            }
            else
            {
                // Set the county resource max storage to equal whatever the player left it as.
                SetResourceMaxValues(resourceStorageHboxDictionary);

                Clock.Instance.UnpauseTime();
                PlayerControls.Instance.playerControlsEnabled = true;
            }
        }

        // This assigns the county's resources to the Storage Hboxes when this becomes visible.
        private void AssignResourcesToStorageHboxes()
        {
            foreach (KeyValuePair<AllEnums.CountyGoodType, GoodData> keyValuePair
                in countyData.goods)
            {
                resourceStorageHboxDictionary[keyValuePair.Key].goodData = keyValuePair.Value;
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
            maxNonperisableAmountAvailableLabel.Text = countyData.nonperishableStorage.ToString();
        }

        // Goes through every resource and counts how much storage is assigned to them.
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
        private static void SetResourceMaxValues(Godot.Collections.Dictionary<AllEnums.CountyGoodType, StorageHbox> storageHboxes)
        {
            foreach (KeyValuePair<AllEnums.CountyGoodType, StorageHbox> keyValuePair in storageHboxes)
            {
                keyValuePair.Value.goodData.MaxAmount = (int)keyValuePair.Value.maxAmountSpinBox.Value;
            }
        }

        // This one needs to differentiate between perishable and nonperishable.
        // It needs to add the label amount so that the player can add more to the spinbox.
        public void UpdateSpinBoxMaxValuePlusLabel()
        {
            foreach (KeyValuePair<AllEnums.CountyGoodType, StorageHbox> keyValuePair in resourceStorageHboxDictionary)
            {
                if (keyValuePair.Value.goodData.perishable == AllEnums.Perishable.Perishable)
                {
                    keyValuePair.Value.maxAmountSpinBox.MaxValue = keyValuePair.Value.maxAmountSpinBox.Value
                            + int.Parse(currentPerishableAvailableLabel.Text);
                }
                else if(keyValuePair.Value.goodData.perishable == AllEnums.Perishable.Nonperishable)
                {
                    keyValuePair.Value.maxAmountSpinBox.MaxValue = keyValuePair.Value.maxAmountSpinBox.Value
                            + int.Parse(currentNonperishableAvailableLabel.Text);
                }
            }
        }

        // This one needs to keep the perishable vs nonperishable.
        public void UpdateCountyAvailableStorageLabels()
        {
            int totalUsedPerishableStorage = 0;
            int totalUsedNonperishableStorage = 0;
            foreach (KeyValuePair<AllEnums.CountyGoodType, StorageHbox> keyValuePair in resourceStorageHboxDictionary)
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

        private void UpdateEachHboxWithResource()
        {
            foreach (KeyValuePair<AllEnums.CountyGoodType, StorageHbox> keyValuePair in resourceStorageHboxDictionary)
            {
                UpdateStorageHboxLabels(keyValuePair);
                //GD.Print(resourceStorageHboxDictionary[keyValuePair.Key].resourceData.MaxAmount);
                resourceStorageHboxDictionary[keyValuePair.Key].maxAmountSpinBox.MaxValue
                    = resourceStorageHboxDictionary[keyValuePair.Key].goodData.MaxAmount;
                resourceStorageHboxDictionary[keyValuePair.Key].maxAmountSpinBox.Value
                    = resourceStorageHboxDictionary[keyValuePair.Key].goodData.MaxAmount;
            }
        }


        private void UpdateStorageHboxLabels(KeyValuePair<AllEnums.CountyGoodType, StorageHbox> keyValuePair)
        {
            resourceStorageHboxDictionary[keyValuePair.Key].goodData = countyData.goods[keyValuePair.Key];
            //GD.Print("Update Storage Hbox Labels Resource:" + countyData.resources[keyValuePair.Key].name);
            resourceStorageHboxDictionary[keyValuePair.Key].resourceNameLabel.Text = $"{Tr(resourceStorageHboxDictionary[keyValuePair.Key].goodData.goodName)}:";
            resourceStorageHboxDictionary[keyValuePair.Key].resourceAmountLabel.Text = resourceStorageHboxDictionary[keyValuePair.Key].goodData.Amount.ToString();
        }

        private void CloseButtonPressed()
        {
            Hide();
        }
    }
}
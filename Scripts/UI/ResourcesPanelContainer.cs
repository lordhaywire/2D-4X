using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

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

        public Godot.Collections.Dictionary<AllEnums.CountyResourceType, StorageHbox> resourceStorageHboxDictionary = [];

        public override void _Ready()
        {
            Instance = this;
            GenerateStorageHBoxes();
        }

        // Creates the Storage Hboxes when the game starts.
        private void GenerateStorageHBoxes()
        {
            foreach (ResourceData resourceData in AllResources.Instance.allResources)
            {
                StorageHbox storageHbox = (StorageHbox)storageHBoxPackedScene.Instantiate();
                GD.Print("Resource Name: " + resourceData.name);
                storageHbox.Name = resourceData.name;
                storageHbox.resourceData = resourceData; // This is strange.

                if (storageHbox.resourceData.perishable)
                {
                    perishableVboxParent.AddChild(storageHbox);
                }
                else
                {
                    nonperishableVboxParent.AddChild(storageHbox);
                }
                resourceStorageHboxDictionary.Add(storageHbox.resourceData.countyResourceType, storageHbox);
            }
        }

        private void OnVisibilityChanged()
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
            foreach (KeyValuePair<AllEnums.CountyResourceType, ResourceData> keyValuePair
                in countyData.resources)
            {
                resourceStorageHboxDictionary[keyValuePair.Key].resourceData = keyValuePair.Value;
            }
        }

        // Updates top max available amount labels, not the spinbox max amounts.
        private void UpdateMaxAvailableStorageAmountLabels()
        {
            currentPerishableAvailableLabel.Text
                = CountAvailableStorageAmounts(countyData.perishableStorage, true).ToString();
            maxPerishableAmountAvailableLabel.Text = countyData.perishableStorage.ToString();

            currentNonperishableAvailableLabel.Text
                = CountAvailableStorageAmounts(countyData.nonperishableStorage, false).ToString();
            maxNonperisableAmountAvailableLabel.Text = countyData.nonperishableStorage.ToString();
        }

        // Goes through every resource and counts how much storage is assigned to them.
        private int CountAvailableStorageAmounts(int maxStorage, bool perishable)
        {
            int storage = 0;
            foreach (ResourceData resourceData in countyData.resources.Values)
            {
                if (resourceData.perishable == perishable)
                {
                    storage += resourceData.MaxAmount;
                }
            }
            int availableStorage = maxStorage - storage;
            return availableStorage;
        }

        // I think the dictionary is in the same order as the resources.
        private static void SetResourceMaxValues(Godot.Collections.Dictionary<AllEnums.CountyResourceType, StorageHbox> storageHboxes)
        {
            foreach (KeyValuePair<AllEnums.CountyResourceType, StorageHbox> keyValuePair in storageHboxes)
            {
                keyValuePair.Value.resourceData.MaxAmount = (int)keyValuePair.Value.maxAmountSpinBox.Value;
            }
        }

        public void UpdateSpinBoxMaxValue(bool perishable)
        {
            UpdateSpinBoxMaxValuePlusLabel(resourceStorageHboxDictionary);
        }


        public void UpdateCountyAvailableStorage()
        {
            // This one needs to keep the perishable vs nonperishable.
            CountyData countyData = Globals.Instance.SelectedLeftClickCounty.countyData;
            int totalUsedPerishableStorage = 0;
            int totalUsedNonperishableStorage = 0;
            foreach (KeyValuePair<AllEnums.CountyResourceType, StorageHbox> keyValuePair in resourceStorageHboxDictionary)
            {
                if (keyValuePair.Value.resourceData.perishable)
                {
                    totalUsedPerishableStorage += (int)keyValuePair.Value.maxAmountSpinBox.Value;
                }
                else
                {
                    totalUsedNonperishableStorage += (int)keyValuePair.Value.maxAmountSpinBox.Value;

                }
                int totalAvailablePerishableStorage = countyData.perishableStorage - totalUsedPerishableStorage;
                currentPerishableAvailableLabel.Text = totalAvailablePerishableStorage.ToString();

                int totalAvailableNonperishableStorage = countyData.nonperishableStorage - totalUsedNonperishableStorage;
                currentNonperishableAvailableLabel.Text = totalAvailableNonperishableStorage.ToString();
            }
        }

        // This has to be the problem!!
        private void UpdateEachHboxWithResource()
        {
            foreach (KeyValuePair<AllEnums.CountyResourceType, StorageHbox> keyValuePair in resourceStorageHboxDictionary)
            {
                resourceStorageHboxDictionary[keyValuePair.Key].resourceData = countyData.resources[keyValuePair.Key];
                //GD.Print("Update Each Type of Resource:" + resources[keyValuePair.Key].name);
                resourceStorageHboxDictionary[keyValuePair.Key].resourceNameLabel.Text = $"{resourceStorageHboxDictionary[keyValuePair.Key].resourceData.name}:";
                resourceStorageHboxDictionary[keyValuePair.Key].resourceAmountLabel.Text = resourceStorageHboxDictionary[keyValuePair.Key].resourceData.amount.ToString();
                // Spinbox
                if (resourceStorageHboxDictionary[keyValuePair.Key].resourceData.perishable)
                {
                    resourceStorageHboxDictionary[keyValuePair.Key].maxAmountSpinBox.MaxValue
                        = countyData.perishableStorage / Globals.Instance.numberOfPerishableResources;
                }
                else
                {
                    resourceStorageHboxDictionary[keyValuePair.Key].maxAmountSpinBox.MaxValue
                        = countyData.nonperishableStorage / Globals.Instance.numberOfNonperishableResources;
                }
                resourceStorageHboxDictionary[keyValuePair.Key].maxAmountSpinBox.Value
                    = resourceStorageHboxDictionary[keyValuePair.Key].resourceData.MaxAmount;
            }
        }

        // This one needs to differentiate between perishable and nonperishable.
        // It needs to get the label amount so that the player can add more to the spinbox.
        private void UpdateSpinBoxMaxValuePlusLabel(
            Godot.Collections.Dictionary<AllEnums.CountyResourceType, StorageHbox> storageHboxes)
        {
            foreach (KeyValuePair<AllEnums.CountyResourceType, StorageHbox> keyValuePair in storageHboxes)
            {
                if (keyValuePair.Value.resourceData.perishable)
                {
                    keyValuePair.Value.maxAmountSpinBox.MaxValue = keyValuePair.Value.maxAmountSpinBox.Value
                            + int.Parse(currentPerishableAvailableLabel.Text);
                }
                else
                {
                    keyValuePair.Value.maxAmountSpinBox.MaxValue = keyValuePair.Value.maxAmountSpinBox.Value
                            + int.Parse(currentNonperishableAvailableLabel.Text);
                }
            }
        }

        private void CloseButtonPressed()
        {
            Hide();
        }
    }
}
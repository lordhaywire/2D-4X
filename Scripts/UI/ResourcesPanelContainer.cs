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
        [Export] private Label perishableLabelSibling;
        [Export] private Label nonperishableLabelSibling;

        public Godot.Collections.Dictionary<AllEnums.CountyResourceType, StorageHbox> resourceStorageHboxDictionary = [];

        public override void _Ready()
        {
            Instance = this;
            GenerateStorageHBoxes();
        }

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
                    perishableLabelSibling.AddSibling(storageHbox);
                }
                else
                {
                    nonperishableLabelSibling.AddSibling(storageHbox);
                }
                resourceStorageHboxDictionary.Add(storageHbox.resourceData.countyResourceType, storageHbox);
            }
        }

        // This assigns the county's resources to the Storage Hboxes when this becomes visible.
        private void AssignResourcesToStorageHboxes()
        {
            County county = Globals.Instance.SelectedLeftClickCounty;
            GD.Print("County: " + county.countyData.countyName);
            foreach (KeyValuePair<AllEnums.CountyResourceType, ResourceData> keyValuePair
                in county.countyData.resources)
            {
                resourceStorageHboxDictionary[keyValuePair.Key].resourceData = keyValuePair.Value;
            }
        }

        private void OnVisibilityChanged()
        {
            if (Visible)
            {
                AssignResourcesToStorageHboxes();
                UpdateMaxAmountLabels();
                UpdateResourceLabels();
                Clock.Instance.PauseandUnpause();
                PlayerControls.Instance.playerControlsEnabled = false;
            }
            else
            {
                // Set the county resource max storage to equal whatever the player left it as.
                SetResourceMaxValues(resourceStorageHboxDictionary);

                Clock.Instance.PauseandUnpause();
                PlayerControls.Instance.playerControlsEnabled = true;
            }
        }

        private static void SetResourceMaxValues(Godot.Collections.Dictionary<AllEnums.CountyResourceType, StorageHbox> storageHboxes)
        {
            foreach (KeyValuePair<AllEnums.CountyResourceType, StorageHbox> keyValuePair in storageHboxes)
            {
                keyValuePair.Value.resourceData.MaxAmount = (int)keyValuePair.Value.maxAmountSpinBox.Value;
            }
        }

        public void UpdateSpinBoxMaxValue(bool perishable)
        {
            UpdateMaxValue(resourceStorageHboxDictionary);
        }

        private void UpdateMaxValue(
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

        public void UpdateCountyAvailableStorage()
        {
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

                int totalAvailableNonperishableStorage = countyData.nonperishableStorage - totalUsedPerishableStorage;
                currentNonperishableAvailableLabel.Text = totalAvailablePerishableStorage.ToString();
            }
           
        }
        private void UpdateResourceLabels()
        {
            countyNameTitleLabel.Text = Globals.Instance.SelectedLeftClickCounty.countyData.countyName;

            UpdateEachHboxWithResource(Globals.Instance.SelectedLeftClickCounty.countyData.resources);
        }

        private void UpdateEachHboxWithResource(Godot.Collections.Dictionary<AllEnums.CountyResourceType, ResourceData> resources)
        {
            int i = 0;
            foreach (KeyValuePair<AllEnums.CountyResourceType, StorageHbox> keyValuePair in resourceStorageHboxDictionary)
            {
                resourceStorageHboxDictionary[keyValuePair.Key].resourceData = resources[keyValuePair.Key];
                GD.Print("Update Each Type of Resource:" + resources[keyValuePair.Key].name);
                resourceStorageHboxDictionary[keyValuePair.Key].resourceNameLabel.Text = $"{resourceStorageHboxDictionary[keyValuePair.Key].resourceData.name}:";
                resourceStorageHboxDictionary[keyValuePair.Key].resourceAmountLabel.Text = resourceStorageHboxDictionary[keyValuePair.Key].resourceData.amount.ToString();
                resourceStorageHboxDictionary[keyValuePair.Key].resourceMaxAmountLabel.Text = resourceStorageHboxDictionary[keyValuePair.Key].resourceData.MaxAmount.ToString();

                // Spinbox
                if (resourceStorageHboxDictionary[keyValuePair.Key].resourceData.perishable)
                {
                    resourceStorageHboxDictionary[keyValuePair.Key].maxAmountSpinBox.MaxValue
                        = Globals.Instance.SelectedLeftClickCounty.countyData.perishableStorage / resources.Count;
                }
                else
                {
                    resourceStorageHboxDictionary[keyValuePair.Key].maxAmountSpinBox.MaxValue
                        = Globals.Instance.SelectedLeftClickCounty.countyData.nonperishableStorage / resources.Count;
                }
                resourceStorageHboxDictionary[keyValuePair.Key].maxAmountSpinBox.Value 
                    = resourceStorageHboxDictionary[keyValuePair.Key].resourceData.MaxAmount;
                i++;
            }
        }

        private void UpdateMaxAmountLabels()
        {
            currentPerishableAvailableLabel.Text
                = CountStorageAmounts(Globals.Instance.SelectedLeftClickCounty.countyData.resources
                , Globals.Instance.SelectedLeftClickCounty.countyData.perishableStorage, true).ToString();
            maxPerishableAmountAvailableLabel.Text
                = Globals.Instance.SelectedLeftClickCounty.countyData.perishableStorage.ToString();
            currentNonperishableAvailableLabel.Text
                = CountStorageAmounts(Globals.Instance.SelectedLeftClickCounty.countyData.resources
                , Globals.Instance.SelectedLeftClickCounty.countyData.nonperishableStorage, false).ToString();
            maxNonperisableAmountAvailableLabel.Text
                = Globals.Instance.SelectedLeftClickCounty.countyData.nonperishableStorage.ToString();
        }

        private static int CountStorageAmounts(Godot.Collections.Dictionary<AllEnums.CountyResourceType, ResourceData> resources
            , int maxStorage, bool perishable)
        {
            int storage = 0;
            foreach (ResourceData resource in resources.Values)
            {
                if (resource.perishable == perishable)
                {
                    storage += resource.MaxAmount;
                }
            }
            int availableStorage = maxStorage - storage;
            return availableStorage;
        }
        private void CloseButtonPressed()
        {
            Hide();
        }
    }
}